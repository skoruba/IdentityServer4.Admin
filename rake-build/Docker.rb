include Rake::DSL

class Docker
  def initialize()
  end

  def tag_and_push(image_name, build_number, registry, org_part)
    registry_image_tagged = "#{registry.url}/#{org_part}#{image_name}:#{build_number}"
    sh "docker tag #{image_name}:#{build_number} #{registry_image_tagged}"
    sh "docker push #{registry_image_tagged}"
    sh "docker rmi #{registry_image_tagged}"
  end
end
