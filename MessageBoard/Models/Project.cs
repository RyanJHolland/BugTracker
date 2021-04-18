using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Models
{
	public class Project
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Please enter a name for this project!")]
		[MaxLength(50)]
		public string Name { get; set; }

		[MaxLength(1000)]
		public string Description { get; set; }

		[Display(Name = "Project Owner ID")]
		public string ProjectOwnerId { get; set; }

		[Display(Name = "Owner")]
		public string ProjectOwnerUserName { get; set; }

		/*
    [Display(Name = "Comment Count")]
    public int CommentCount { get; set; }

    [Display(Name = "Last Comment")]
    public int LastCommentId { get; set; }
    */
		/*
     * I'm not sure if I should query for these things whenever I load the page versus storing them here.
     * Storing them when someone tickets seems more efficient.
     * But what if someone edits the ticket title after it is added to this Project table?

    [Display(Name = "Ticket Count")]
    public int totalTicketsInQuery { get; set; }

    [Display(Name = "Newest Ticket Time")]
    public DateTime NewestTicketTime { get; set; }

    [Display(Name = "Newest Ticket Title")]
    public int NewestTicketTitle { get; set; }

    [Display(Name = "Newest Comment Time")]
    public DateTime NewestCommentTime { get; set; }

    [Display(Name = "Newest Comment Was To")]
    public int NewestCommentWasTo { get; set; }
    */
	}
}
