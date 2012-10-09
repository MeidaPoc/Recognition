using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


/// <summary>
/// Desingned to remove the training a EigenObjectRecognizer code from the main form
/// </summary>
class Classifier_Train: IDisposable
{

    #region Variables

    //Eigen
    MCvTermCriteria termCrit;
    EigenObjectRecognizer recognizer;

    //training variables
    List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();//Images
    List<string> Names_List = new List<string>(); //labels
    int ContTrain, NumLabels;

    //Class Variables
    string Error;
    bool _IsTrained = false;

    #endregion

    #region Constructors
    /// <summary>
    /// Default Constructor, Looks in (Application.StartupPath + "\\TrainedFaces") for traing data.
    /// </summary>
    public Classifier_Train()
    {
        termCrit = new MCvTermCriteria(ContTrain, 0.001);
        _IsTrained = LoadTrainingData(Application.StartupPath + "\\TrainedFaces");
    }

    /// <summary>
    /// Takes String input to a different location for training data
    /// </summary>
    /// <param name="Training_Folder"></param>
    public Classifier_Train(string Training_Folder)
    {
        termCrit = new MCvTermCriteria(ContTrain, 0.001);
        _IsTrained = LoadTrainingData(Training_Folder);
    }
    #endregion

    #region Public
    /// <summary>
    /// <para>Return(True): If Training data has been located and Eigen Recogniser has been trained</para>
    /// <para>Return(False): If NO Training data has been located of error in training has occured</para>
    /// </summary>
    public bool IsTrained
    {
        get { return _IsTrained; }
    }

    /// <summary>
    /// Recognise a Grayscale Image using the trained Eigen Recogniser
    /// </summary>
    /// <param name="Input_image"></param>
    /// <returns></returns>
    public string Recognise(Image<Gray, byte> Input_image)
    {
        if (_IsTrained)
        {
            string t = recognizer.Recognize(Input_image);
            return t;
        }
        else return "";//Blank prefered else can use null
    
    }

    /// <summary>
    /// Returns a string contatining any error that has occured
    /// </summary>
    public string Get_Error
    {
        get { return Error; }
    }

    /// <summary>
    /// Dispose of Class call Garbage Collector
    /// </summary>
    public void Dispose()
    {
        recognizer = null;
        trainingImages = null;
        Names_List = null;
        Error = null;
        GC.Collect();
    }

    #endregion

    #region Private
    /// <summary>
    /// Loads the traing data given a (string) folder location
    /// </summary>
    /// <param name="Folder_loacation"></param>
    /// <returns></returns>
    private bool LoadTrainingData(string Folder_loacation)
    {
        if (File.Exists(Folder_loacation +"\\TrainedLabels.xml"))
        {
            try
            {
                //message_bar.Text = "";
                Names_List.Clear();
                trainingImages.Clear();
                FileStream filestream = File.OpenRead(Folder_loacation + "\\TrainedLabels.xml");
                long filelength = filestream.Length;
                byte[] xmlBytes = new byte[filelength];
                filestream.Read(xmlBytes, 0, (int)filelength);
                filestream.Close();

                MemoryStream xmlStream = new MemoryStream(xmlBytes);

                using (XmlReader xmlreader = XmlTextReader.Create(xmlStream))
                {
                    while (xmlreader.Read())
                    {
                        if (xmlreader.IsStartElement())
                        {
                            switch (xmlreader.Name)
                            {
                                case "NAME":
                                    if (xmlreader.Read())
                                    {
                                        Names_List.Add(xmlreader.Value.Trim());
                                        NumLabels += 1;
                                    }
                                    break;
                                case "FILE":
                                    if (xmlreader.Read())
                                    {
                                        //PROBLEM HERE IF TRAININGG MOVED
                                        trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "\\TrainedFaces\\" + xmlreader.Value.Trim()));
                                    }
                                    break;
                            }
                        }
                    }
                }
                ContTrain = NumLabels;

                if (trainingImages.ToArray().Length != 0)
                {
                    //Eigen face recognizer
                    recognizer = new EigenObjectRecognizer(trainingImages.ToArray(),
                    Names_List.ToArray(), 5000, ref termCrit); //5000 default
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
                return false;
            }
        }
        else return false;
    }
    #endregion
}

