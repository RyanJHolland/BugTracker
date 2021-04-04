using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class ProjectBugsViewModel
	{
		public Project Project { get; set; }
		public List<Bug> Bugs { get; set; }

		/*
		 * orderBy:	category by which to sort the tickets
		 * order: ASC or DESC
		 */
		public string orderBy { get; set; }
		public string order { get; set; }
	}
}