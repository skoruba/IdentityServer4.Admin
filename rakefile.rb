require "./rake-build/DockerRegistry.rb"
require "./rake-build/Docker.rb"
require "./rake-build/Build.rb"

is_windows = Gem.win_platform?
puts "Running on windows: #{is_windows}"
build_number = ENV['BUILD_NUMBER'] || "0.1.0"
tenant = ENV["TENANT"] || "Demo"
build = Build.new()
docker = Docker.new()
registry_gcr = DockerRegistry.new("ghcr.io", is_windows)

task :version do
  if build_number
    puts "Will use the: #{build_number} build number provided by TeamCity"
  else
    puts "Will use the hardcoded build number: #{build_number}"
  end
end

task :build => [:version] do
  build.project("./src/Skoruba.IdentityServer4.STS.Identity/Dockerfile", "ezy.identityserver.sts.identity", build_number)
  build.project("./src/Skoruba.IdentityServer4.Admin/Dockerfile", "ezy.identityserver.admin", build_number)
end

task :docker_registry_login do
  registry_gcr.login()
end

task :docker_registry_logout do
  registry_gcr.logout()
end

task :images_push_to_GCR => [:version, :docker_registry_login] do
  docker.tag_and_push("ezy.identityserver.sts.identity", build_number, registry_gcr, "ezywebwerkstaden/")
  docker.tag_and_push("ezy.identityserver.admin", build_number, registry_gcr, "ezywebwerkstaden/")
end

task :bundle__push_to_GCR => [ :docker_registry_login, :images_push_to_GCR, :docker_registry_logout]
task :default => :build
