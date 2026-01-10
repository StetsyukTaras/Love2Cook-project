namespace Love2Cook.API.DTOs {
    public class CreateReviewDto {
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public byte Rating { get; set; }  // 1-5
        public string? Comment { get; set; }
    }
}