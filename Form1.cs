using LibraryActivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryDatabse
{
    public partial class Form : System.Windows.Forms.Form
    {
        string title;
        string author;
        string genre;
        TranslatedBook translateBook = null;
        AudioBook audioBook;
        GroupBox[] groups;

        public Form()
        {
            InitializeComponent();
        }
        private void generalSubmit_Click(object sender, EventArgs e)
        {
            if (titleTB.Text == "" || authorTB.Text == "" || genreTB.Text == "")
            {
                msgLbl.Text = "Enter all fields";
                return;
            }

            title = titleTB.Text;
            author = authorTB.Text;
            genre = genreTB.Text;

            radioGroup.Visible = true;
        }

        private void radioButton_Checked(object sender, EventArgs e)
        {
            //no {} to save space
            if (sender == regularRadio)
                groupVisibility(regularGroup);

            else if (sender == translateRadio)
                groupVisibility(translateGroup);

            else if (sender == audioRadio)
                groupVisibility(audioGroup);

            else
                groupVisibility(childrensGroup);
        }

        private void groupVisibility(GroupBox box)
        {
            groups = new GroupBox[] { regularGroup, translateGroup, audioGroup, childrensGroup, summaryGroup, illustratorGroup };

            foreach (GroupBox item in groups)
            {
                if (item == box)
                {
                    item.Visible = true;
                    continue;
                }
                item.Visible = false;
            }
        }

        private void groupSubmit_Click(object sender, EventArgs e)
        {
            bool tmp;

            GroupBox visibleBox = null;
            ChildrensBook childBook = null;

            foreach (GroupBox item in groups)
            {
                if (item.Visible == true)
                {
                    visibleBox = item;
                    break;
                }
            }

            if (visibleBox == regularGroup)
            {
                groupSubmit.Location = new Point(332, 477);
                cancelBtn.Location = new Point(253, 477);

                if (availableTB.Text == "")
                    groupLbl.Text = "Enter all fields";
                else
                {
                    tmp = validateYN(availableTB.Text);

                    if (tmp)
                    {
                        tmp = checkTrueFalse(availableTB.Text); // returns true / false

                        Book regularBook = new Book(title, author, genre, tmp);

                        msgLbl.Text = "Regular book created!";
                    }
                }
            }

            else if (visibleBox == translateGroup)
            {
                groupSubmit.Location = new Point(373, 550);
                cancelBtn.Location = new Point(296, 550);

                if (ogLanguageTB.Text == "")
                    groupLbl.Text = "Enter all fields";
                else
                    translateBook = new TranslatedBook(title, author, genre, ogLanguageTB.Text);
            }

            else if (visibleBox == audioGroup)
            {
                groupSubmit.Location = new Point(373, 550);
                cancelBtn.Location = new Point(296, 550);

                if (durationTB.Text == "" || nameTB.Text == "" || formatTB.Text == "")
                    groupLbl.Text = "Enter all fields";
                else
                {
                    audioBook = new AudioBook(title, author, genre, Convert.ToSingle(durationTB.Text), nameTB.Text, formatTB.Text);

                    msgLbl.Text = "Audio book created!";

                    groupVisibility(summaryGroup);
                }
            }

            else if (visibleBox == summaryGroup)
            {
                groupSubmit.Location = new Point(332, 477);
                cancelBtn.Location = new Point(253, 477);

                if (summaryTB.Text == "")
                    msgLbl.Text = "Fill summary field";
                else
                {
                    audioBook.addSummary(summaryTB.Text);
                    summaryGroup.Visible = false;
                }
            }

            else if (visibleBox == childrensGroup)
            {
                if (startAgeTB.Text == "" || endAgeTB.Text == "" || illustrationsTB.Text == "" || interactiveTB.Text == "")
                    msgLbl.Text = "Fill summary field";
                else
                {
                    tmp = validateYN(illustrationsTB.Text);
                    bool tmp2 = validateYN(interactiveTB.Text);

                    if (tmp && tmp2)
                    {
                        tmp = checkTrueFalse(availableTB.Text); // returns true / false
                        tmp2 = checkTrueFalse(interactiveTB.Text);

                        childBook = new ChildrensBook(title, author, genre, (startAgeTB.Text + " - " + endAgeTB.Text), tmp, tmp2);
                    }
                }
            }

            else if (visibleBox == illustratorGroup)
            {
                groupSubmit.Location = new Point(332, 477);
                cancelBtn.Location = new Point(253, 477);

                if (illustNameTB.Text == "")
                    msgLbl.Text = "Fill name field";
                else
                {
                    childBook.addIllustrator(illustNameTB.Text);
                }
            }
        }

        private bool validateYN(string text)
        {
            if (availableTB.Text != "Y" && availableTB.Text != "y" && availableTB.Text != "N" && availableTB.Text != "n")
            {
                msgLbl.Text = "Enter Y or N";
                return false;
            }
            return true;
        }

        private bool checkTrueFalse(string answer)
        {
            if (answer == "y" || answer == "Y")
            {
                return true;
            }
            return false;
        }

        private void translateAdd_Click(object sender, EventArgs e)
        {
            if (translateBook == null)
            {
                groupLbl.Text = "No book created yet";
                return;
            }
            if (newLanguageTB.Text == "")
                groupLbl.Text = "Enter a language";
            else
                translateBook.addTranslation(newLanguageTB.Text);
        }
    }
}
