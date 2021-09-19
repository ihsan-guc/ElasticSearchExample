using ElasticSearchExample.Entites.Entities;
using System;
using System.Collections.Generic;

namespace ElasticSearchExample.Web.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
            Persons = new List<Person>();
        }
        public List<Person> Persons{ get; set; }
    }
}
