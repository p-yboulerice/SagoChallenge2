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
		
		# the array of symbols
		debug_symbols = ['SAGO_GOOGLE_PLAY_EXPANSION_DOWNLOADER']
		
		# read the project settings from ProjectSettings.asset
		project_settings_string = File.read(project_settings_path)
		project_settings_symbols = debug_symbols.select do |symbol|
			project_settings_string.include? symbol
		end
		
		# fail if any symbols are found in the project settings
		if project_settings_symbols.length > 0
			GitHook::log "The Unity project settings contain the following debug define symbols:"
			GitHook::log project_settings_symbols.uniq
			GitHook::log "Please remove them before committing."
			GitHook::log "You cannot commit google play and google play free platform prefabs with"
			GitHook::log " the .Use Expansion File. checked."
			exit 1
		end
		
	end
	
}