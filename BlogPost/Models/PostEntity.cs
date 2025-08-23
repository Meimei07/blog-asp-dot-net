using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPost.Models
{
    public class PostEntity
    {
        public int Id {get;set;}

        public string Title {get;set;}

        public string Content {get;set;}

        public string Thumbnail {get;set;}

        // one to many: fk
        public int CategoryEntityId {get;set;}

        // navigation
        public CategoryEntity Category {get;set;}

        // navigation many to many
        public ICollection<TagEntity> Tags {get;set;}

        [NotMapped]
        public IFormFile ImgFile {get;set;}
    }
}
