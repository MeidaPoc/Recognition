using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.GPU;
using Emgu.CV.Structure;
using Emgu.Util;

namespace RecognitionV3
{
    public partial class FormMain : Form
    {
        #region variables
        Image<Bgr, Byte> currentFrame;
        Image<Gray, Byte> result, TrainedFace = null;
        //Image<Gray, Byte> gray_frame = null;

        long detectionTime;
        List<Rectangle> faces = new List<Rectangle>();

        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5);

        //Classifier with default training location
        Classifier_Train Eigen_Recog = new Classifier_Train();

        Capture grabber = new Capture();
        #endregion

        public FormMain()
        {
            InitializeComponent();
            initialise_capture();            
        }


        private void Detection(object r, EventArgs e)
        {
            currentFrame = grabber.QueryFrame();
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            DetectFace.Detect(currentFrame, "haarcascade_frontalface_default.xml", faces, out detectionTime);
            foreach (Rectangle face in faces)
            {    //result = currentFrame.Copy(face.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                currentFrame.Draw(face, new Bgr(Color.Red), 2);

                //Get copy of img and show it
                result = currentFrame.Copy(face).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC); //making small copy of face
                result._EqualizeHist();                
                if (Eigen_Recog.IsTrained)
                {
                    string name = Eigen_Recog.Recognise(result);
                    //Draw the label for each face detected and recognized
                    currentFrame.Draw(name, ref font, new Point(face.X - 2, face.Y - 2), new Bgr(Color.LightGreen));                    
                }
            }

            //display the image 
            ImageViewer.Image = currentFrame;
            labelTimeSpend.Text = detectionTime.ToString() + "msec";

            faces.Clear();
            currentFrame.Dispose();
        }

        private void trainingToolStripMenuItem_Click(object sender, EventArgs e)
        {  
            //Stop Camera
            stop_capture();

            //OpenForm
            Training_Form TF = new Training_Form(this);
            TF.Show();
        }

        private void stop_capture()
        {
            Application.Idle -= Detection;
        }

        internal void initialise_capture()
        {
            Application.Idle += Detection;
        }

        public void retrain()
        {
            Eigen_Recog = new Classifier_Train();
            if (Eigen_Recog.IsTrained)
            {
                labelmessege.Text = "Training Data loaded";
            }
            else
            {
                labelmessege.Text = "No training data found, please train program using Train menu option";
            }
        }
    }
}
