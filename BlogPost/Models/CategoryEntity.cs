namespace BlogPost.Models
{
    public class CategoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // navigation one to many
        public ICollection<PostEntity> Posts {get;set;}
    }
}
