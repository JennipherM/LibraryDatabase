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
using System.Data.SqlClient;
using System.Net;
using System.Xml.Linq;

namespace LibraryDatabse
{
    public partial class Form : System.Windows.Forms.Form
    {
        string title;
        string author;
        string genre;

        GroupBox[] groups;
        List<string> languages = new List<string>();
        
        bool tmp;
        bool illustrations;
        bool interactive;
        string bookType;

        string connectionString;
        SqlConnection currConnection;
        SqlCommand otherTblQuery = null;
        string otherStatement = "";

        public Form()
        {
            InitializeComponent();
            connectionString = @"Data Source = localhost; Initial Catalog = Books; User ID = LibraryUser; Password = LibraryLab123!; TrustServerCertificate = True";
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
            string lang = "";
            int bookID;
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
                    tmp = validateYN(availableTB.Text); //check if text is y or n

                    if (tmp)
                    {
                        tmp = checkTrueFalse(availableTB.Text); // returns true (y) / false (n)

                        currConnection = new SqlConnection(connectionString);
                        currConnection.Open();

                        switch (bookType)
                        {
                            case "Regular":
                                Book regularBook = new Book(title, author, genre, tmp);
                                bookInsert();
                                break;

                            case "Translated":
                                TranslatedBook translateBook = new TranslatedBook(title, author, genre, tmp, ogLanguageTB.Text);

                                translateBook.addTranslation(languages);

                                foreach (string language in languages)
                                {
                                    lang += language + ", ";
                                }

                                languages.Clear();

                                bookInsert(); //inserts new record into Books tbl

                                bookID = getBookID(); //gets ID of book just entered into Books

                                otherStatement = "INSERT INTO dbo.TranslatedBooks(BookID, OriginalLanguage, AvailableLanguages) VALUES (@BookID, @OriginalLanguage, @AvailableLanguages)";

                                otherTblQuery = new SqlCommand(otherStatement, currConnection);

                                otherTblQuery.Parameters.AddWithValue("@BookID", bookID);
                                otherTblQuery.Parameters.AddWithValue("@OriginalLanguage", ogLanguageTB.Text);
                                otherTblQuery.Parameters.AddWithValue("@AvailableLanguages", lang);

                                otherTblQuery.ExecuteNonQuery();
                                break;

                            case "Audio":

                                AudioBook audioBook = new AudioBook(title, author, genre, tmp, durationTB.Text, nameTB.Text, formatTB.Text);

                                if (summaryTB.Text != "")
                                {
                                    audioBook.addSummary(summaryTB.Text);
                                }

                                bookInsert();
                                bookID = getBookID();

                                otherStatement = "INSERT INTO dbo.AudioBooks(BookID, Duration, NarratorName, FormatType, Summary) VALUES (@BookID, @Duration, @NarratorName, @FormatType, @Summary)";

                                otherTblQuery = new SqlCommand(otherStatement, currConnection);

                                otherTblQuery.Parameters.AddWithValue("@BookID", bookID);
                                otherTblQuery.Parameters.AddWithValue("@Duration", durationTB.Text);
                                otherTblQuery.Parameters.AddWithValue("@NarratorName", nameTB.Text);
                                otherTblQuery.Parameters.AddWithValue("@FormatType", formatTB.Text);
                                otherTblQuery.Parameters.AddWithValue("@Summary", audioBook.summary);

                                otherTblQuery.ExecuteNonQuery();
                                summaryGroup.Visible = false;
                                break;

                            case "Childrens":
                                ChildrensBook childBook = new ChildrensBook(title, author, genre, tmp, (startAgeTB.Text + " - " + endAgeTB.Text), illustrations, interactive);

                                if (illustNameTB.Text != "")
                                {
                                    childBook.addIllustrator(illustNameTB.Text);
                                }

                                bookInsert();
                                bookID = getBookID();

                                otherStatement = "INSERT INTO dbo.ChildrensBooks(BookID, AgeRange, Illustrations, IllustratorName, Interactive) VALUES (@BookID, @AgeRange, @Illustrations, @IllustratorName, @Interactive)";

                                otherTblQuery = new SqlCommand(otherStatement, currConnection);

                                otherTblQuery.Parameters.AddWithValue("@BookID", bookID);
                                otherTblQuery.Parameters.AddWithValue("@AgeRange", startAgeTB.Text + " - " + endAgeTB.Text);
                                otherTblQuery.Parameters.AddWithValue("@Illustrations", illustrations);
                                otherTblQuery.Parameters.AddWithValue("@IllustratorName", childBook.illustratorName);
                                otherTblQuery.Parameters.AddWithValue("@Interactive", interactive);

                                otherTblQuery.ExecuteNonQuery();
                                break;
                        }

                        msgLbl.Text = $"{bookType} book created!";
                        cancelBtn_Click(availableGroup, e);
                        currConnection.Close();
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
                {
                    groupLbl.Text = "Enter all fields";
                    return;
                }
                try
                {
                    Convert.ToSingle(durationTB.Text);
                }
                catch
                {
                    groupLbl.Text = "Duration must be a number";
                    return;
                }

                groupVisibility(summaryGroup);
                noSumBtn.Visible = true;
                changeBtnLocation(336, 494, 259, 494);
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
                int start;
                int end;

                if (startAgeTB.Text == "" || endAgeTB.Text == "" || illustrationsTB.Text == "" || interactiveTB.Text == "")
                {
                    msgLbl.Text = "Fill summary field";
                    return;
                }
                try
                {
                    start = Convert.ToInt32(startAgeTB.Text);
                    end = Convert.ToInt32(endAgeTB.Text);
                }
                catch
                {
                    groupLbl.Text = "Start and end age must be a number";
                    return;
                }

                if (start > 17)
                {
                    groupLbl.Text = "Childrens books must be age appropriate for children under 18";
                    return ;
                }
                else if (start >end || start == end)
                {
                    groupLbl.Text = "Start age must be less than end age";
                    return;
                }

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
                newLanguageTB.Text = "";
                groupSubmit.Enabled = true;
            }
        }

        private void viewBtn_Click(object sender, EventArgs e)
        {
            string availability;
            BookList.Items.Clear();

            currConnection = new SqlConnection(connectionString);
            currConnection.Open();

            string sqlStatement = "SELECT * FROM dbo.Books";

            SqlCommand myQuery = new SqlCommand(sqlStatement, currConnection);

            SqlDataReader myReader = myQuery.ExecuteReader();

            while (myReader.Read())
            {
                BookList.Items.Add("Title: " + myReader["Title"]);
                BookList.Items.Add("Author: " + myReader["Author"]);
                BookList.Items.Add("Genre: " + myReader["Genre"]);
                BookList.Items.Add("Type: " + myReader["Type"]);

                if (Convert.ToInt16(myReader["Available"]) == 0)
                {
                    availability = "False";
                }
                else
                {
                    availability = "True";
                }

                BookList.Items.Add("Available: " + availability);
                BookList.Items.Add("-------------------------");
            }

            currConnection.Close();
        }

        private void bookInsert()
        {
            SqlCommand bookTblQuery = null;

            // only insert into Book table
            string bookStatement = "INSERT INTO dbo.Books (Title, Author, Genre, Type, Available) VALUES (@Title, @Author, @Genre, @Type, @Available)";

            bookTblQuery = new SqlCommand(bookStatement, currConnection);

            bookTblQuery.Parameters.AddWithValue("@Title", title);
            bookTblQuery.Parameters.AddWithValue("@Author", author);
            bookTblQuery.Parameters.AddWithValue("@Genre", genre);
            bookTblQuery.Parameters.AddWithValue("@Type", bookType);
            bookTblQuery.Parameters.AddWithValue("@Available", tmp);

            bookTblQuery.ExecuteNonQuery();
        }

        private int getBookID()
        {
            //to connect tables by bookID (will insert bookid in other tables instead of auto increment)
            otherStatement = "SELECT BookID FROM Books WHERE Title = @Title AND Author = @Author";

            otherTblQuery = new SqlCommand(otherStatement, currConnection);
            otherTblQuery.Parameters.AddWithValue("@Title", title);
            otherTblQuery.Parameters.AddWithValue("@Author", author);

            //ExecuteScalar() returns a value from db
            return Convert.ToInt16(otherTblQuery.ExecuteScalar());
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