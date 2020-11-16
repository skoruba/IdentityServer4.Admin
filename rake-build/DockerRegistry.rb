include Rake::DSL

class DockerRegistry
  attr_reader :url

  def initialize(url, is_windows)
    @url = url
    @is_windows = is_windows
  end

  def login
    non_printable_env_var_cmd = @is_windows ? "%GCR_PAT%" : "$GCR_PAT"
    sh "echo #{non_printable_env_var_cmd} | docker login #{@url} -u ezydeploy --password-stdin"
  end

  def logout
    sh "docker logout #{@url}"
  end
end
