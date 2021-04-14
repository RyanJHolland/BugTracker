using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public class Bug
	{
		public int Id { get; set; }

		[MinLength(1)]
		[MaxLength(100)]
		[Required(ErrorMessage = "Please enter a title for this bug.")]
		[Display(Name = "Title")]
		public string Title { get; set; }

		[MaxLength(5000)]
		[Display(Name = "Details")]
		public string Body { get; set; }

		[Display(Name = "User Id")]
		public string UserId { get; set; }

		[Display(Name = "Owner")]
		public string UserName { get; set; }

		[Display(Name = "Created")]
		public DateTime CreationTime { get; set; }

		[Display(Name = "Project Id")]
		public int ParentProjectId { get; set; }

		[Required(ErrorMessage = "Please choose a priority.")]
		public PriorityEnum Priority { get; set; }

		public enum PriorityEnum
		{
			Critical,
			High,
			Medium,
			Low
		}

		[Required(ErrorMessage = "Please choose a category.")]
		[Display(Name = "Category")]
		public CategoryEnum Category { get; set; }

		public enum CategoryEnum
		{
			Bug,
			Feature,
			Style,
			Change
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

		public Bug(UserManager<IdentityUser> userManager)
		{
				_userManager = userManager;

				CreationTime = DateTime.UtcNow;
				//UserId = User.GetUserId();
		}
		*/
	}
}