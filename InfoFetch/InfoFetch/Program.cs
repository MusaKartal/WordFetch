using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Net.Http;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

public class Word
{

    static void Main(string[] args)
    {
        Words();
    }


    static void Words()
    {
        var html = @"https://www.englishcentral.com/blog/adan-zye-en-cok-kullanilan-ingilizce-kelimeler-ve-anlamlari/";

        HtmlWeb web = new HtmlWeb();

        var htmlDoc = web.Load(html);

        HtmlNodeCollection rows = htmlDoc.DocumentNode.SelectNodes("//tr");
        if (rows != null)
        {
            string connectionString = "Data Source=MUSA\\SQLEXPRESS;Initial Catalog=EnglishWord;Integrated Security=True;";
            // Veritabanı bağlantısı ve tablo oluşturma
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //CreateTable(connection);

                foreach (HtmlNode row in rows)
                {
                    HtmlNodeCollection columns = row.SelectNodes(".//th | .//td");

                    if (columns != null && columns.Count >= 3)
                    {
                        string english = columns[0].InnerText.Trim();
                        string turkish = columns[1].InnerText.Trim();
                        string turkishPronunciations = columns[2].InnerText.Trim();

                        // Veritabanına ekleme
                        try
                        {
                            using (SqlCommand command = new SqlCommand("INSERT INTO Words (English, Turkish, TurkishPronunciations) VALUES (@English, @Turkish, @TurkishPronunciations)", connection))
                            {
                                command.Parameters.AddWithValue("@English", english);
                                command.Parameters.AddWithValue("@Turkish", turkish);
                                command.Parameters.AddWithValue("@TurkishPronunciations", turkishPronunciations);
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("add data error: " + ex.Message);
                            // Hata durumunda gerekli işlemleri yapabilirsiniz.
                        }
                    }
                }
                Console.WriteLine("the transaction is complete");
                connection.Close();
            }
        }
    }

    static void CreateTable(SqlConnection connection)
    {
        string createTableQuery =  "CREATE TABLE Words (Id int IDENTITY(1,1) PRIMARY KEY, English nvarchar(50), Turkish nvarchar(100), TurkishPronunciations nvarchar(100));";
        using (SqlCommand command = new SqlCommand(createTableQuery, connection))
        {
            command.ExecuteNonQuery();
        }
    }
}