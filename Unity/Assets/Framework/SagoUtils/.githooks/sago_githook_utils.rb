class GitHookUtils

	# Find path to "ProjectSettings/ProjectSettings.asset".
	# This is usually in our Unity/ subfolder, but not always.
	def self.get_project_settings_path(project_root)
		
		project_settings_file = "ProjectSettings/ProjectSettings.asset"

		if (!project_root.end_with?("/"))
			project_root = "#{project_root}/"
		end
		
		# look for default first - much faster
		default_project_settings_path = "#{project_root}Unity/#{project_settings_file}"
		if (File.file?(default_project_settings_path))
			project_settings_path = default_project_settings_path
		else
			search_results = Dir.glob("#{project_root}**/#{project_settings_file}")
			if (search_results.count > 0)
				project_settings_path = search_results[0]
			else
				project_settings_path = nil
			end
		end

		unless project_settings_path.nil?
			project_settings_path = project_settings_path.gsub(project_root, "")  # strip out source path
		end

		return project_settings_path
	end

end
