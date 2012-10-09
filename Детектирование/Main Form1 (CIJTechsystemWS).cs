using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;

using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace Face_Recognition
{
    public partial class Form1 : Form
    {
        #region variables
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        public HaarCascade Face;

        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5);

        int ContTrain, NumLabels, t;
        string name, names = null;
        List<string> Names_List = new List<string>(); //labels

        List<string> NamePersons = new List<string>();
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray_frame = null;

        #endregion
        public Form1()
        {
            InitializeComponent();
            //create Cascades
            Face = new HaarCascade(Application.StartupPath + "/Cascades/haarcascade_frontalface_alt2.xml");//haarcascade_frontalface_alt_tree.xml");

            //Load of previus trainned faces and labels for each image
            if (LoadTrainingData())
            {
                message_bar.Text = "Training Data loaded";
            }
            else
            {
                message_bar.Text = "No training data found, please train program using Train menu option";
            }
            initialise_capture();
            
        }

        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Stop Camera
            stop_capture();
  
            //OpenForm
            Training_Form TF = new Training_Form(this);
            TF.Show();
        }

        public void initialise_capture()
        {
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            Application.Idle += new EventHandler(FrameGrabber);
        }
        private void stop_capture()
        {
            Application.Idle -= new EventHandler(FrameGrabber);
            if(grabber!= null)
            {
            grabber.Dispose();
            }
            //Initialize the FrameGraber event
        }

        void FrameGrabber(object sender, EventArgs e)
        {
            NamePersons.Add("");
            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();

                //Face Detector
                MCvAvgComp[][] facesDetected = gray_frame.DetectHaarCascade(Face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(30, 30));

                //Action for each element detected
                foreach (MCvAvgComp face_found in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(face_found.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    //draw the face detected in the 0th (gray) channel with blue color
                    currentFrame.Draw(face_found.rect, new Bgr(Color.Red), 2);

                    if (trainingImages.ToArray().Length != 0)
                    {
                        //TermCriteria for face recognition with numbers of trained images like maxIteration
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                        //Eigen face recognizer
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                           trainingImages.ToArray(),
                           Names_List.ToArray(),
                           5000,
                           ref termCrit);

                        name = recognizer.Recognize(result);

                        //Draw the label for each face detected and recognized
                        currentFrame.Draw(name, ref font, new Point(face_found.rect.X - 2, face_found.rect.Y - 2), new Bgr(Color.LightGreen));

                    }

                    NamePersons[t - 1] = name;
                    NamePersons.Add("");



                }
                t = 0;

                //Names concatenation of persons recognized
                for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                {
                    names = names + NamePersons[nnn] + ", ";
                }
                //Show the faces procesed and recognized
                image_PICBX.Image = currentFrame.ToBitmap();
                //label4.Text = names;
                names = "";
                //Clear the list(vector) of names
                NamePersons.Clear();

            }
        }

        public bool LoadTrainingData()
        {
            if (File.Exists(Application.StartupPath + "/TrainedFaces/TrainedLabels.xml"))
            {
                try
                {
                    message_bar.Text = "";
                    Names_List.Clear();
                    trainingImages.Clear();
                    FileStream filestream = File.OpenRead(Application.StartupPath + "/TrainedFaces/TrainedLabels.xml");
                    long filelength = filestream.Length;
                    byte[] xmlBytes = new byte[filelength];
                    filestream.Read(xmlBytes, 0, (int)filelength);
                    filestream.Close();

                    MemoryStream xmlStream = new MemoryStream(xmlBytes);

                    using (XmlReader xmlreader = XmlTextReader.Create(xmlStream))
                    {
                        int i = 0;
                        string val = null;
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
                                            trainingImages.Add(new Image<Gray, byte>(xmlreader.Value.Trim()));
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    ContTrain = NumLabels;

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else return false;
        }

        System.Windows.Forms.Timer t_mer = new System.Windows.Forms.Timer();
    }
}
