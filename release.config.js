module.exports = {
    verifyConditions: [
        // Verifies a NugetApiKey environment variable has been provided
        () => {
            if (!process.env.NugetApiKey) {
                throw new SemanticReleaseErrorc(
                    'The environment variable NugetApiKey is required.',
                    'ENOAPMTOKEN',
                )
            }
        },
        // Verifies the conditions for the plugins used below
        // For example verifying a GITHUB_TOKEN environment variable has been provided
        '@semantic-release/changelog',
        '@semantic-release/git',
        '@semantic-release/github',
        // '@semantic-release/exec'
    ],
    publish: [
        // https://github.com/semantic-release/changelog
        // Set of semantic-release plugins for creating or updating a changelog file.
        // '@semantic-release/changelog',

        // https://github.com/semantic-release/git
        // Git plugin is need so the changelog file will be committed to the Git repository and available on subsequent builds in order to be updated.
        // '@semantic-release/git',

        // https://github.com/semantic-release/git
        // Exec plugin uses to call dotnet nuget push to push the packages from 
        // the artifacts folder to NuGet
        {
            path: '@semantic-release/exec',
            cmd: `dotnet nuget push .\\artifacts\\*.nupkg -k ${process.env.NugetApiKey} -s https://api.nuget.org/v3/index.json`,
        },
        
        // https://github.com/semantic-release/github
        // Set of Semantic-release plugins for publishing a GitHub release.
        // Includes the packages from the artifacts folder as assets
        {
            path: '@semantic-release/github',
            assets: 'artifacts/**/*.nupkg',
        }
    ]
}