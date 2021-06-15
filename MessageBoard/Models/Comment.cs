using System;
using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Models
{
	public class Comment
	{
		public int Id { get; set; }

		[MaxLength(5000)]
		public string Body { get; set; }

		[Display(Name = "User Id")]
		public string UserId { get; set; }

		[Display(Name = "User")]
		public string UserName { get; set; }

		[Display(Name = "Creation Time")]
		[DisplayFormat(DataFormatString = "{0:MMM d, yyyy h:mm tt K}")]
		public DateTime CreationTime { get; set; }

		public int ParentTicketId { get; set; }

		public int ParentCommentId { get; set; }
	}
}
