using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace YATDL
{
    public class TaskModel : ModelBase
    {
        public TaskModel()
        {
        }

        public TaskModel(Task t)
        {
            this.Id = t.Id;
            this.Name = t.Name;
            this.Done = t.Done;
            this.Created = t.Created;
            this.Importance = t.Importance.ToString();
            this.Description = t.Description;
        }

        [JsonProperty("name")]
        [Display(Name = "Имя")]
        [StringLength(255)]
        public string Name { get; set; }

        [JsonProperty("created")]
        [Display(Name = "Дата создания")]
        public System.DateTime Created { get; set; }

        [JsonProperty("description")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [JsonProperty("importance")]
        [Display(Name = "Важность")]
        public string Importance { get; set; }

        [HiddenInput]
        public bool? Done { get; set; }

        [JsonProperty("done")]
        [Display(Name = "Выполнен")]
        public bool NoNullDone
        {
            get { return Done ?? false; }
            set { Done = value; }
        }
    }
}