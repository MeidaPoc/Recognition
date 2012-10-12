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
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Face_Recognition
{
    public partial class Form1 : Form
    {
        #region variables
        Image<Bgr, Byte> currentFrame;
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray_frame = null;

        List<InfoFounded> founded = new List<InfoFounded>();
        List<Image<Gray, Byte>> AllObjects = new List<Image<Gray, byte>>();
        InfoFounded current_info;

        Capture grabber;
        DetectionV2 detect2 = new DetectionV2();

        public HaarCascade Face = new HaarCascade(Application.StartupPath + "/Cascades/haarcascade_frontalface_alt2.xml");//haarcascade_frontalface_alt_tree.xml");

        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5);

        int NumLabels;
        float PersentOfIdentity = 0.60F;

        //Classifier with default training location
        Classifier_Train Eigen_Recog = new Classifier_Train();

        Classifier_Train_Objects Obj_Recog = new Classifier_Train_Objects();
        #endregion

        public Form1()
        {
            InitializeComponent();

            //Load of previus trainned faces and labels for each image

            if (Eigen_Recog.IsTrained)
            {
                message_bar.Text = "Training Data loaded";
            }
            else
            {
                message_bar.Text = "No training data found, please train program using Train menu option";
            }
            initialise_capture();
            
        }

        //Open training form and pass this
        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Stop Camera
            stop_capture();

            //OpenForm
            Training_Form TF = new Training_Form(this);
            TF.Show();
        }
        public void retrain()
        {
            Eigen_Recog = new Classifier_Train();
            if (Eigen_Recog.IsTrained)
            {
                message_bar.Text = "Training Data loaded";
            }
            else
            {
                message_bar.Text = "No training data found, please train program using Train menu option";
            }
        }

        //Camera Start Stop
        public void initialise_capture()
        {
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            Application.Idle += new EventHandler(FrameGrabber_Parrellel);
        }

        private void stop_capture()
        {
            Application.Idle -= new EventHandler(FrameGrabber_Parrellel);
            if(grabber!= null)
            {
            grabber.Dispose();
            }
        }

        //Process Frame
        void FrameGrabber_Standard(object sender, EventArgs e)
        {
            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();

                //Face Detector
                MCvAvgComp[][] facesDetected = gray_frame.DetectHaarCascade(Face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(50, 50));

                //Action for each element detected
                foreach (MCvAvgComp face_found in facesDetected[0])
                {
                    result = currentFrame.Copy(face_found.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    //draw the face detected in the 0th (gray) channel with blue color
                    currentFrame.Draw(face_found.rect, new Bgr(Color.Red), 2);

                    if (Eigen_Recog.IsTrained)
                    {
                        string name = Eigen_Recog.Recognise(result);
                        //Draw the label for each face detected and recognized
                        currentFrame.Draw(name, ref font, new Point(face_found.rect.X - 2, face_found.rect.Y - 2), new Bgr(Color.LightGreen));
                        ADD_Face_Found(result, name);
                    }
                }
                //Show the faces procesed and recognized
                image_PICBX.Image = currentFrame.ToBitmap();
            }
        }

        void FrameGrabber_Parrellel(object sender, EventArgs e)
        {
            founded.Clear();
            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            //Clear_Faces_Found();

            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();

                //Face Detector
                MCvAvgComp[][] facesDetected = gray_frame.DetectHaarCascade(Face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(50, 50));

                FindObjects(gray_frame);//foreach  // for each image in base make MatchTemplate

                //Action for each element detected                
                Parallel.ForEach(facesDetected[0], face_found =>
                    {
                        try
                        {
                            
                            result = currentFrame.Copy(face_found.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                            result._EqualizeHist();
                            //draw the face detected in the 0th (gray) channel with blue color
                            //currentFrame.Draw(face_found.rect, new Bgr(Color.Red), 2);

                            if (Eigen_Recog.IsTrained)
                            {
                                string name = Eigen_Recog.Recognise(result);
                                //Draw the label for each face detected and recognized
                                //currentFrame.Draw(name, ref font, new Point(face_found.rect.X - 2, face_found.rect.Y - 2), new Bgr(Color.LightGreen));
                                ADD_Face_Found(result, name);
                                founded.Add(new InfoFounded(0, name, face_found.rect));
                            }
                            else
                            {
                                founded.Add(new InfoFounded(0, " ", face_found.rect));
                            }
                        }
                        catch
                        {
                            //do nothing as parrellel loop buggy
                            //No action as the error is useless, it is simply an error in 
                            //no data being there to process and this occurss sporadically 
                        }
                    });
                Drawing();
                //Show the faces procesed and recognized
                image_PICBX.Image = currentFrame.ToBitmap();
            }
        }

        private void Drawing()
        {
            foreach (InfoFounded found in founded)
            {
                currentFrame.Draw(found.Name, ref font, new Point(found.x - 2, found.y - 2), new Bgr(Color.LightGreen));
                //currentFrame.Draw(new Rectangle(found.x, found.y,found.x+2, found.y+1), new Bgr(Color.LightGreen),2);
            }
        }

        private void FindObjects(Image<Gray, byte> gray_frame)
        {
            //Image<Gray, Byte> sourceImage = new Image<Gray, Byte>(@"Images/Source.bmp");
            //Image<Gray, Byte> templateImage = new Image<Gray, Byte>(@"Images/Template.bmp");
            Image<Gray, float> resultImage;// = sourceImage.MatchTemplate(templateImage, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED);
            List<InfoFounded> list = new List<InfoFounded>();
            InfoFounded current_founded; 
            Rectangle rect;
            int width, height;
                        
            double[] min, max;
            double minVal, maxVal, minLoc, maxLoc;
            Point[] point1, point2;
            //resultImage.MinMax(out min, out max, out point1, out point2);
            Point point3 = new Point();
            //point3.X = point1[0].X + templateImage.Width;
            //point3.Y = point1[0].Y + templateImage.Height;
            MCvScalar scalar = new MCvScalar(255);
            //CvInvoke.cvRectangle(sourceImage.Ptr, point1[0], point3, scalar, 5, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, 0);

            //this.pictureBox1.Image = sourceImage.ToBitmap();
            foreach (Image<Gray, Byte> img in Obj_Recog.trainingImages)
            {
                current_founded = null;                
                width = (int)gray_frame.Size.Width - (int)img.Size.Width + 1;
                height = (int)gray_frame.Size.Height - (int)img.Size.Height + 1;
                resultImage = new Image<Gray, float>(width, height);
                //resultImage = gray_frame.MatchTemplate(img, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED);
                Emgu.CV.CvInvoke.cvMatchTemplate(gray_frame, img, resultImage, Emgu.CV.CvEnum.TM_TYPE.CV_TM_SQDIFF_NORMED);
                //Emgu.CV.CvInvoke.cvMinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc, IntPtr.Zero);
                resultImage.MinMax(out min, out max, out point1, out point2);
                int forcopyX1=point1[0].X;
                int forcopyY1=point1[0].Y;
                int forcopyWidth = img.Size.Width;
                int forcopyHeight = img.Size.Height;
                Rectangle testcopy = new Rectangle(forcopyX1,forcopyY1,forcopyWidth,forcopyHeight);
                pictureBox2.Image = gray_frame.Copy(testcopy).ToBitmap();
                if(detect2.Detect_object(gray_frame, img))
                {
                    pictureBox3.Image = gray_frame.Copy(new Rectangle(detect2.Location,img.Size)).ToBitmap();
                }
                
                /*float[,,] matches = resultImage.Data;
                for (int x = 0; x < matches.GetLength(1); x++)
                {
                    for (int y = 0; y < matches.GetLength(0); y++)
                    {
                        double matchScore = matches[y, x, 0];
                        if (matchScore > PersentOfIdentity)
                        {
                            rect = new Rectangle(new Point(x, y), new Size(img.Width, img.Height));
                            current_founded = new InfoFounded(0, " ", rect);
                            labelcontrol.Text = x + ":" + y;
                            pictureBox2.Image = gray_frame.Copy(rect).ToBitmap();
                            
                            //currentFrame.Draw("Obj", ref font, new Point(x - 2,y - 2), new Bgr(Color.LightGreen));                            
                        }
                    }
                }
                resultImage.MinMax(out min, out max, out point1, out point2);
                point3.X = point1[0].X + img.Width;
                point3.Y = point1[0].Y + img.Height;
                if (!(current_founded == null))
                {
                    list.Add(current_founded);
                }*/

                pictureBox1.Image = img.ToBitmap();                
                //CvInvoke.cvRectangle(gray_frame.Ptr, point1[0], point3, scalar, 5, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, 0);
            }
        }

        #region var for panel
        //ADD Picture box and label to a panel for each face
        int faces_count = 0;
        int faces_panel_Y = 0;
        int faces_panel_X = 0;
        #endregion

        void Clear_Faces_Found()
        {
            this.Faces_Found_Panel.Controls.Clear();
            faces_count = 0;
            faces_panel_Y = 0;
            faces_panel_X = 0;
            founded.Clear();
        }

        void ADD_Face_Found(Image<Gray, Byte> img_found, string name_person)
        {
            if (""==name_person)
            {
                return ;                
            }
            current_info = IsInPanel(name_person);
            if (!(null == current_info))
                {
                    this.Faces_Found_Panel.Controls.RemoveAt(current_info.indexOfPix);
                    PictureBox PI = new PictureBox();
                    PI.Location = new Point(current_info.x, current_info.y);
                    PI.Height = 80;
                    PI.Width = 80;
                    PI.SizeMode = PictureBoxSizeMode.StretchImage;
                    PI.Image = img_found.ToBitmap();
                    this.Faces_Found_Panel.Controls.Add(PI);
                    current_info.indexOfPix = this.Faces_Found_Panel.Controls.IndexOf(PI);
                }
                else
                {
                    PictureBox PI = new PictureBox();
                    PI.Location = new Point(faces_panel_X, faces_panel_Y);
                    PI.Height = 80;
                    PI.Width = 80;
                    PI.SizeMode = PictureBoxSizeMode.StretchImage;
                    PI.Image = img_found.ToBitmap();
                    Label LB = new Label();
                    LB.Text = name_person;
                    LB.Location = new Point(faces_panel_X, faces_panel_Y + 80);
                    LB.Width = 50;
                    LB.Height = 15;

                    this.Faces_Found_Panel.Controls.Add(PI);
                    this.Faces_Found_Panel.Controls.Add(LB);
                    current_info = new InfoFounded(this.Faces_Found_Panel.Controls.IndexOf(PI),name_person,faces_panel_X,faces_panel_Y);
                    founded.Add(current_info);

                    faces_count++;
                    /*if (faces_count == 5)
                    {
                        faces_panel_X = 0;
                        faces_panel_Y += 100;
                        faces_count = 0;
                    }
                    else*/
                    faces_panel_X += 85;

                    if (Faces_Found_Panel.Controls.Count > 14)
                    {
                        Clear_Faces_Found();
                    }
                }
        }

        private InfoFounded IsInPanel(string name_person)
        {
            foreach(InfoFounded found in founded)
                if (name_person == (found.Name))
                {
                    return found;
                }
            return null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            Clear_Faces_Found();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void labelcontrol_Click(object sender, EventArgs e)
        {

        }
        
    }
}
