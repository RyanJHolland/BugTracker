using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class ProjectBugsViewModel
	{
		public Project Project { get; set; }
		public List<Bug> Bugs { get; set; }
	}
}