using BugTracker.Models;

namespace BugTracker.ViewModels
{
	public class ProjectBugCommentViewModel
	{
		public Project Project { get; set; }
		public Bug Bug { get; set; }
		public Comment Comment { get; set; }
	}
}