using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public class Project
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Please enter a name for this project!")]
		[MaxLength(50)]
		public string Name { get; set; }

		[MaxLength(1000)]
		public string Description { get; set; }

		/*

    [Display(Name = "Comment Count")]
    public int CommentCount { get; set; }

    [Display(Name = "Last Comment")]
    public int LastCommentId { get; set; }
    */
		/*
     * I'm not sure if I should query for these things whenever I load the page versus storing them here.
     * Storing them when someone bugs seems more efficient.
     * But what if someone edits the bug title after it is added to this Project table?

    [Display(Name = "Bug Count")]
    public int BugCount { get; set; }

    [Display(Name = "Newest Bug Time")]
    public DateTime NewestBugTime { get; set; }

    [Display(Name = "Newest Bug Title")]
    public int NewestBugTitle { get; set; }

    [Display(Name = "Newest Comment Time")]
    public DateTime NewestCommentTime { get; set; }

    [Display(Name = "Newest Comment Was To")]
    public int NewestCommentWasTo { get; set; }
    */
	}
}