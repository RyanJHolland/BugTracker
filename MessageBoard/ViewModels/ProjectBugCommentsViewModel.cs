using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class ProjectBugCommentsViewModel
	{
		public Project Project { get; set; }
		public Bug Bug { get; set; }
		public List<Comment> Comments { get; set; }
	}
}