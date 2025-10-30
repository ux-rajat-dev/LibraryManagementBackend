using System;
using System.Collections.Generic;

namespace LibraryManagement.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? AuthorId { get; set; }

    public int? GenreId { get; set; }

    public string? Isbn { get; set; }

    public int? TotalCopies { get; set; }

    public int? AvailableCopies { get; set; }

    public int? PublishedYear { get; set; }

    public string? CoverImageUrl { get; set; }

    public virtual Author? Author { get; set; }

    public virtual ICollection<Borrowtransaction> Borrowtransactions { get; set; } = new List<Borrowtransaction>();

    public virtual Genre? Genre { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
