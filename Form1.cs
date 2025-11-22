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

        GroupBox[] groups;

        List<string> languages = new List<string>();

        bool illustrations;
        bool interactive;
        string bookType; 

        public Form()
        {
            InitializeComponent();
        }
        private void generalSubmit_Click(object sender, EventArgs e)
        {
            msgLbl.Text = "";

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
            groupSubmit.Visible = true;
            cancelBtn.Visible = true;

            if (sender == regularRadio)
            {
                groupVisibility(availableGroup);
                changeBtnLocation(336, 494, 259, 494);
                bookType = "Regular";
            }
            else if (sender == translateRadio)
            {
                groupVisibility(translateGroup);
                changeBtnLocation(336, 494, 259, 494);
                bookType = "Translated";
            }
            else if (sender == audioRadio)
            {
                groupVisibility(audioGroup);
                changeBtnLocation(373, 550, 296, 550);
                bookType = "Audio";
            }
            else
            {
                groupVisibility(childrensGroup);
                changeBtnLocation(429, 589, 352, 589);
                bookType = "Childrens";
            }
        }

        // makes one group box visible, others hidden
        private void groupVisibility(GroupBox box)
        {
            groups = new GroupBox[] { availableGroup, translateGroup, audioGroup, childrensGroup, summaryGroup, illustratorGroup, languageGroup };

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
            GroupBox visibleBox = null;
            groupLbl.Text = "";

            foreach (GroupBox item in groups)
            {
                if (item.Visible == true)
                {
                    visibleBox = item;
                    break;
                }
            }

            // -------- Regular Book --------

            if (visibleBox == availableGroup)
            {
                if (availableTB.Text == "")
                    groupLbl.Text = "Enter all fields";
                else
                {
                    bool tmp = validateYN(availableTB.Text); //check if text is y or n

                    if (tmp)
                    {
                        tmp = checkTrueFalse(availableTB.Text); // returns true (y) / false (n)

                        switch (bookType)
                        {
                            case "Regular":
                                Book regularBook = new Book(title, author, genre, tmp);
                                msgLbl.Text = "Regular book created!";
                                break;

                            case "Translated":
                                TranslatedBook translateBook = new TranslatedBook(title, author, genre, tmp, ogLanguageTB.Text);

                                translateBook.addTranslation(languages);
                                msgLbl.Text = "Translated book created!";

                                languages.Clear();
                                break;

                            case "Audio":
                                AudioBook audioBook = new AudioBook(title, author, genre, tmp, durationTB.Text, nameTB.Text, formatTB.Text);

                                msgLbl.Text = "Audio book created!";

                                if(summaryTB.Text != "")
                                {
                                    audioBook.addSummary(summaryTB.Text);
                                }
                                summaryGroup.Visible = false;
                                break;

                            case "Childrens":

                                ChildrensBook childBook = new ChildrensBook(title, author, genre, tmp,(startAgeTB.Text + " - " + endAgeTB.Text), illustrations, interactive);

                                if(illustNameTB.Text != "")
                                {
                                    childBook.addIllustrator(illustNameTB.Text);
                                }

                                msgLbl.Text = "Childrens book created!";
                                break;
                        }
                        cancelBtn_Click(availableGroup, e);
                    }
                }              
            }

            // -------- Translated Book --------
            else if (visibleBox == translateGroup)
            {
                if (ogLanguageTB.Text == "")
                {
                    groupLbl.Text = "Enter original language";
                    return;
                }
                else
                {
                    languages.Clear();
                    groupVisibility(languageGroup);
                    changeBtnLocation(340, 529, 263, 529);
                    groupSubmit.Enabled = false;
                }               
            }
            else if (visibleBox == languageGroup)
            {
                groupVisibility(availableGroup);               
            }

            // -------- Audio Book --------
            else if (visibleBox == audioGroup)
            {
                if (durationTB.Text == "" || nameTB.Text == "" || formatTB.Text == "")
                    groupLbl.Text = "Enter all fields";
                else
                {
                    groupVisibility(summaryGroup);
                    noSumBtn.Visible = true;
                    changeBtnLocation(336, 494, 259, 494);
                }
            }

            else if (visibleBox == summaryGroup)
            {
                if (summaryTB.Text == "")
                    groupLbl.Text = "Fill summary field";
                else
                    groupVisibility(availableGroup);
            }

            // -------- Childrens Book --------
            else if (visibleBox == childrensGroup)
            {
                if (startAgeTB.Text == "" || endAgeTB.Text == "" || illustrationsTB.Text == "" || interactiveTB.Text == "")
                    msgLbl.Text = "Fill summary field";
                else
                {
                    // check if input is y or n
                    illustrations = validateYN(illustrationsTB.Text);
                    interactive = validateYN(interactiveTB.Text);

                    if (illustrations && interactive)
                    {
                        illustrations = checkTrueFalse(illustrationsTB.Text); // returns true(y) / false(n)
                        interactive = checkTrueFalse(interactiveTB.Text);

                        if (illustrations)
                        {
                            groupVisibility(illustratorGroup);
                            changeBtnLocation(336, 494, 259, 494);
                        }
                        else
                            groupVisibility(availableGroup);
                    }
                }
            }

            else if (visibleBox == illustratorGroup)
            {
                if (illustNameTB.Text == "")
                    groupLbl.Text = "Fill name field";
                else
                    groupVisibility(availableGroup);
            }
        }
        private void translateAdd_Click(object sender, EventArgs e)
        {
            if (newLanguageTB.Text == "")
            {
                groupLbl.Text = "Enter a language";
                return;
            }
            else
            {
                languages.Add(newLanguageTB.Text);

                groupLbl.Text = $"{newLanguageTB.Text} added to list!";
                groupSubmit.Enabled = true;
            }
        }

        private void viewBtn_Click(object sender, EventArgs e)
        {

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            regularRadio.Checked = false;
            translateRadio.Checked = false;
            audioRadio.Checked = false;
            childrensRadio.Checked = false;

            TextBox[] box = new TextBox[] {titleTB, authorTB, genreTB, availableTB, durationTB, endAgeTB, formatTB, illustNameTB, illustrationsTB, interactiveTB, nameTB, newLanguageTB, ogLanguageTB, startAgeTB, summaryTB};

            foreach (GroupBox item in groups)
            {
                item.Visible = false;
            }

            foreach(TextBox tb in box)
            {
                tb.Text = "";
            }

            radioGroup.Visible = false;
            groupSubmit.Visible = false;
            cancelBtn.Visible = false;
            noSumBtn.Visible = false;

            groupLbl.Text = "";
            groupSubmit.Enabled = true;
        }

        //no summary btn
        private void noSumBtn_Click(object sender, EventArgs e)
        {
            groupVisibility(availableGroup);
        }

        //changes submit / cancel btns positions
        private void changeBtnLocation(int submitX, int sumbitY, int cancelX, int cancelY)
        {
            groupSubmit.Location = new Point(submitX, sumbitY);
            cancelBtn.Location = new Point(cancelX, cancelY);
        }
        private bool validateYN(string text)
        {
            if (text != "Y" && text != "y" && text != "N" && text != "n")
            {
                groupLbl.Text = "Enter Y or N";
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
    }
}



/* ASSIGNMENT
    To complete this exercise, utilize a database and associated table to store values from your Library application.

    You current application should be modified in the following way:

    - Have a graphical interface for entering data. The required fields will depend on your material types / classes from your prior projects.

    - Have a database backend that contains the materials as records. This can be one or more tables, depending on how complex you have made your materials.

    - Note: While a well formed relational database would be ideal, I will accept a non-relational single table or other solution.

    - After any library material is created as an object, you should store the relevant information into a database table.

    When complete, submit your project as a link to your GitHub, or as an attachment. Be sure to save your application for future use.
 */