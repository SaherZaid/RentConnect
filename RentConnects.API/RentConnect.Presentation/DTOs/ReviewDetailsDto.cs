namespace RentConnect.API.RentConnect.Presentation.DTOs
{
    public class ReviewDetailsDto
    {
        public Guid ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDto User { get; set; }
        public ItemDto Item { get; set; }

        public class UserDto
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
        }

        public class ItemDto
        {
            public Guid ItemId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal PricePerDay { get; set; }
        }
    }
}