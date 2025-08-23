namespace BlogPost.Models
{
    public class TagEntity
    {
        public int Id {get;set;}

        public string Name {get;set;}

        // navigation many to many
        public ICollection<PostEntity> Posts {get;set;}
    }
}
