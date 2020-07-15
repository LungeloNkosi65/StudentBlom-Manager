using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Configuration;
using System.Web;
using StudentBloCosmo.ViewModel;

namespace StudentBloCosmo.Service
{
    public class CosmodbBussines
    {
        private string endpointUrl;
        private string primaryKey;
        private DocumentClient client;
        public CosmodbBussines()
        {
            endpointUrl = ConfigurationManager.AppSettings["EndpointUrl"];
            primaryKey = ConfigurationManager.AppSettings["PrimaryKey"];
            client = new DocumentClient(new Uri(endpointUrl), primaryKey);
        }

        public async Task<List<Student>> StudentsList(string stName)
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "ToDoList" });

            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("ToDoList"),
                new DocumentCollection { Id = "Items" });


            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };


            IQueryable<Student> studentQuery = this.client.CreateDocumentQuery<Student>(
                    UriFactory.CreateDocumentCollectionUri("ToDoList", "Items"), queryOptions)
                    .Where(f => f.studentName != "");
            if (!String.IsNullOrEmpty(stName))
            {
                studentQuery = studentQuery.Where(m => m.studentName.Contains(stName) && m.IsActive == false);
            }
            string[] kl = new string[25];
            BlobBussines blob = new BlobBussines("picture");
            int i = 0;
            foreach (var item in blob.BlobList())
            {
                kl[i] = item;
                i = i + 1;
            }
            i = 0;
            List<Student> students = new List<Student>();
            foreach (var item in studentQuery)
            {
                Student studente = new Student();
                studente.imageUri = item.imageUri;
                studente.IsActive = item.IsActive;
                studente.mobile = item.mobile;
                studente.IsDeleted = item.IsDeleted;
                studente.studentName = item.studentName;
                studente.studentNumber = item.studentNumber;
                studente.surname = item.surname;
                studente.telphone_No = item.telphone_No;
                studente.email = item.email;
                studente.Id = item.Id;
                students.Add(studente);
                i = i + 1;
            }

            students = students.Where(m => m.IsDeleted == false).ToList();
            return students;
        }
        public async Task<List<Student>> DeletedStudentsList(string stName)
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "ToDoList" });

            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("ToDoList"),
                new DocumentCollection { Id = "Items" });


            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };


            IQueryable<Student> studentQuery = this.client.CreateDocumentQuery<Student>(
                    UriFactory.CreateDocumentCollectionUri("ToDoList", "Items"), queryOptions)
                    .Where(f => f.studentName != "");
            if (!String.IsNullOrEmpty(stName))
            {
                studentQuery = studentQuery.Where(m => m.studentName.Contains(stName) && m.IsActive == false);
            }
            string[] kl = new string[25];
            BlobBussines blob = new BlobBussines("picture");
            int i = 0;
            foreach (var item in blob.BlobList())
            {
                kl[i] = item;
                i = i + 1;
            }
            i = 0;
            List<Student> students = new List<Student>();
            foreach (var item in studentQuery)
            {
                Student studente = new Student();
                studente.imageUri = kl[i];
                studente.IsActive = item.IsActive;
                studente.mobile = item.mobile;
                studente.studentName = item.studentName;
                studente.studentNumber = item.studentNumber;
                studente.surname = item.surname;
                studente.telphone_No = item.telphone_No;
                studente.email = item.email;
                studente.Id = item.Id;
                students.Add(studente);
                i = i + 1;

            }
            students = students.Where(m => m.IsDeleted == true).ToList();
            return students;
        }

        public async Task AddStudent(HttpPostedFileBase uploadFile, Student student)
        {
            BlobBussines BlobObj = new BlobBussines("picture");
            string FileAbsoluteUri = BlobObj.UploadFile(uploadFile);
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Student> studentQuery = this.client.CreateDocumentQuery<Student>(
                    UriFactory.CreateDocumentCollectionUri("ToDoList", "Items"), queryOptions)
                    .Where(f => f.studentName != "");

            Student Studentz = new Student();
            student.imageUri = FileAbsoluteUri;
            student.studentNumber = 1;
            if (studentQuery != null)
            {
                foreach (var item in studentQuery.ToList())
                {
                    Studentz.studentNumber = item.studentNumber;
                }
                student.studentNumber = Studentz.studentNumber + 1;
            }
            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("ToDoList", "Items"), student);
        }
        public Student EditStudent(string DocumentId)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Student> studentQuery = this.client.CreateDocumentQuery<Student>(
                    UriFactory.CreateDocumentCollectionUri("ToDoList", "Items"), queryOptions)
                    .Where(f => f.Id == DocumentId);

            Student Studentz = new Student();
            if (studentQuery != null)
            {
                foreach (var item in studentQuery.ToList())
                {

                    Studentz.Id = item.Id;
                    Studentz.studentNumber = item.studentNumber;
                    Studentz.studentName = item.studentName;
                    Studentz.surname = item.surname;
                    Studentz.email = item.email;
                    Studentz.telphone_No = item.telphone_No;
                    Studentz.mobile = item.mobile;
                    Studentz.IsActive = item.IsActive;

                }
            }
            return Studentz;
        }

        public async Task Deactivate(string documentId)
        {
            Student Studentz = getStudent(documentId);
            Studentz.IsActive = true;
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("ToDoList", "Items", Studentz.Id), Studentz, null);
        }

        public async Task DeleteStudent(string documentId)
        {
            Student Studentz = getStudent(documentId);
            Studentz.IsDeleted = true;
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("ToDoList", "Items", Studentz.Id), Studentz, null);
        }
        public async Task UpdateAsync(Student Studentz)
        {
           await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("ToDoList", "Items", Studentz.Id), Studentz, null);
        }

        private Student getStudent(string DocumentId) 
        {
            Student studentz = new Student();
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
            IQueryable<Student> studentQuery = this.client.CreateDocumentQuery<Student>(
                    UriFactory.CreateDocumentCollectionUri("ToDoList", "Items"), queryOptions)
                    .Where(f => f.Id == DocumentId);

            if (studentQuery != null)
            {
                foreach (var item in studentQuery.ToList())
                {
                    studentz.Id = item.Id;
                    studentz.studentNumber = item.studentNumber;
                    studentz.studentName = item.studentName;
                    studentz.surname = item.surname;
                    studentz.email = item.email;
                    studentz.telphone_No = item.telphone_No;
                    studentz.mobile = item.mobile;
                    studentz.imageUri = item.imageUri;
                    studentz.IsActive = item.IsActive;
                    studentz.IsDeleted = item.IsDeleted;
                }
            }
            return studentz;
        }
    }
}
