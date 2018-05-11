GitHook::add :pre_commit, 0, Proc.new {
		
	# define the submodule path
	submodule_path = Pathname.new(__FILE__).realpath.dirname.dirname
	
	# define the project settings path
	project_settings_path = GitHookUtils.get_project_settings_path(".")
	if project_settings_path.nil?
		GitHook::log "Failed to find project settings path"
		exit 1
	end
	
	# define the project settings status
	project_settings_staged = %x( git diff --cached --name-only ).include? project_settings_path
	
	# if the project settings are staged
	if project_settings_staged
		
		# read the array of symbols from Platform.cs
		platform_path = File.join(submodule_path, "Scripts/Platform.cs")
		platform_string = File.open(platform_path, "r:UTF-8") { |file| file.read }
		platform_symbol_pattern = /Platform\.\w+\s*,\s*["']([A-Z0-9_]+)["']/
		platform_symbols = platform_string.scan platform_symbol_pattern
		platform_symbols = platform_symbols.map { |matches| matches.first }
		
		# read the project settings from ProjectSettings.asset
		# TODO: what if ProjectSettings.asset is binary?
		project_settings_string = File.read(project_settings_path)
		project_settings_symbols = platform_symbols.select do |symbol|
			project_settings_string.include? symbol
		end
		
		# fail if any symbols are found in the project settings
		if project_settings_symbols.length > 0
			GitHook::log "The Unity project settings contain platform define symbols:"
			GitHook::log project_settings_symbols.uniq
			GitHook::log "Please remove them before committing."
			exit 1
		end
		
	end
	
}