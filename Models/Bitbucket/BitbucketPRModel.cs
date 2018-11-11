using System;
using System.Collections.Generic;

namespace Q.Squid.Models.Bitbucket
{
    public class PullRequest
    {
        public string title { get; set; }
        public string description { get; set; }
        public Target source { get; set; }
        public Target destination { get; set; }
        public List<Author> reviewers { get; set; } = null;
        public bool close_source_branch { get; set; } = false;
        public object merge_commit { get; set; }
        public string state { get; set; }
        public object closed_by { get; set; }
        public Summary summary { get; set; }
        public int comment_count { get; set; }
        public Author author { get; set; }
        public DateTime created_on { get; set; }
        public string reason { get; set; }
        public DateTime updated_on { get; set; }
        public string type { get; set; }
        public int id { get; set; }
        public int task_count { get; set; }
    }

    public class Summary
    {
        public string raw { get; set; }
        public string markup { get; set; }
        public string html { get; set; }
        public string type { get; set; }
    }

    public class Author
    {
        public string username { get; set; }
        public string display_name { get; set; }
        public string type { get; set; }
        public string uuid { get; set; }
    }

    public class Repository
    {
        public string type { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string uuid { get; set; }
    }

    public class Target
    {
        public Commit commit { get; set; }
        public Repository repository { get; set; }
        public Branch branch { get; set; }
    }

    public class Commit
    {
        public string hash { get; set; }
    }

    public class Branch
    {
        public string name { get; set; }
    }

    public class PullRequestsResponse
    {
        public int pagelen { get; set; }
        public List<PullRequest> values { get; set; }
        public int page { get; set; }
        public int size { get; set; }
    }

    public class PullRequestStatus
    {
        public string key { get; set; }
        public string description { get; set; }
        public Repository repository { get; set; }
        public string url { get; set; }
        public string refname { get; set; }
        public string state { get; set; }
        public DateTime created_on { get; set; }
        public Commit commit { get; set; }
        public DateTime updated_on { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }

    public class PullRequestStatusesResponse
    {
        public int pagelen { get; set; }
        public List<PullRequestStatus> values { get; set; }
        public int page { get; set; }
        public int size { get; set; }
    }

    public class MergePullRequestModel
    {
        public string type { get; set; }
        public string message { get; set; }
        public bool close_source_branch { get; set; }
        public string merge_strategy { get; set; }
    }
}