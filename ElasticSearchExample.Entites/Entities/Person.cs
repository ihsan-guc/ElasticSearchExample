using System;

namespace ElasticSearchExample.Entites.Entities
{
    public class Person : BaseGuidEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ImagePath { get; set; }
        public bool IsElastic { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
