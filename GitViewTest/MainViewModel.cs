using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGit2Sharp;

namespace GitViewTest
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            using (var repo = new Repository(@"<Your Git repository>"))
            {
                var nodes = repo.Commits.QueryBy(new CommitFilter { SortBy = CommitSortStrategies.Time|CommitSortStrategies.Topological }).ToArray();

                Commits = nodes.Select(n => new CommitViewModel
                {
                    Id = n.Id.Sha,
                    ParentIds = n.Parents.Select(p => p.Id.Sha).ToArray(),
                    Author = n.Author.Name,
                    Message = n.MessageShort.Trim()
                }).ToArray();
            }
        }

        public CommitViewModel[] Commits { get; }
    }

    public class CommitViewModel
    {
        public string Id { get; set; }
        public string[] ParentIds { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
    }
}
