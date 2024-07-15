using LibGit2Sharp;

namespace WebApplication2.Service;

public class GitService
{
    private readonly string _repoPath;

    public string RepositoryPath => _repoPath;

    public GitService(string repoPath)
    {
        _repoPath = repoPath;
        if (!Repository.IsValid(repoPath))
        {
            Repository.Init(repoPath);
        }
    }

    public void AddAndCommit(string filePath, string message)
    {
        using (var repo = new Repository(_repoPath))
        {
            Commands.Stage(repo, filePath);
            Signature author = new Signature("author", "email@example.com", DateTime.Now);
            Signature committer = author;

            repo.Commit(message, author, committer);
        }
    }
    
    public void UpdateAndCommit(string filePath, string message)
    {
        using (var repo = new Repository(_repoPath))
        {
            Commands.Stage(repo, filePath);
            Signature author = new Signature("author", "email@example.com", DateTime.Now);
            Signature committer = author;

            repo.Commit(message, author, committer);
        }
    }
    
    public IEnumerable<Commit> GetCommitHistory(string fileName)
    {
        using (var repo = new Repository(_repoPath))
        {
            foreach (var commit in repo.Commits)
            {
                if (commit.Tree[fileName] != null)
                {
                    yield return commit;
                }
            }
        }
    }
    
    public string GetFileContentByVersion(string filePath, string commitSha)
    {
        using (var repo = new Repository(_repoPath))
        {
            var commit = repo.Lookup<Commit>(commitSha);
            var relativePath = filePath.Replace(_repoPath, "").TrimStart(Path.DirectorySeparatorChar);
            var treeEntry = commit[relativePath];
            var blob = (Blob)treeEntry.Target;

            using (var reader = new StreamReader(blob.GetContentStream(), true))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
