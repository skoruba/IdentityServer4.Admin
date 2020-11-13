include Rake::DSL

class Build
  def project(dockerfile, image_name, build_number)
    sh "docker build -t #{image_name}:#{build_number} -f #{dockerfile} --build-arg version=#{build_number} ."
  end
end
