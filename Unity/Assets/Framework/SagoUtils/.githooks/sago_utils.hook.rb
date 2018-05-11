GitHook::add :pre_commit, 0, Proc.new {
	GitHook::debug "Hello, World!"
}