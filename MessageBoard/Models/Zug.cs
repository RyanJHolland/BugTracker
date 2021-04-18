using System;
using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Models
{
	public class Zug
	{
		public int Id { get; set; }

		[MinLength(1)]
		[MaxLength(100)]
		[Required(ErrorMessage = "Please enter a title for this ticket.")]
		[Display(Name = "Ticket")]
		public string Title { get; set; }

		[MaxLength(5000)]
		public string Body { get; set; }

		[Display(Name = "User Id")]
		public string UserId { get; set; }

		[Display(Name = "User")]
		public string UserName { get; set; }

		[Display(Name = "Creation Time")]
		public DateTime CreationTime { get; set; }

		[Display(Name = "Project Id")]
		public int ParentProjectId { get; set; }

		public PriorityEnum Priority { get; set; }

		public enum PriorityEnum
		{
			Critical,
			High,
			Medium,
			Low
		}

		[Display(Name = "Type")]
		public CategoryEnum Category { get; set; }

		public enum CategoryEnum
		{
			Ticket,
			Feature,
			Style,
			Change,
			Other
		}

		public StatusEnum Status { get; set; }

		public enum StatusEnum
		{
			Open,
			Resolved,
			Cancelled
		}

		/*
				[Display(Name = "Comment Count")]
				public int CommentCount { get; set; }

				[Display(Name = "Last Comment")]
				public int LastCommentId { get; set; }
		*/
		/*
		private readonly UserManager<IdentityUser> _userManager;

		public Ticket(UserManager<IdentityUser> userManager)
		{
				_userManager = userManager;

				CreationTime = DateTime.UtcNow;
				//UserId = User.GetUserId();
		}
		*/
	}
}
