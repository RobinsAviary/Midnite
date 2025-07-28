namespace Midnite;

public class Directories
{
    // Allows us to access the folder name *and* the full path in one object.

    public Dir resources = new();
    public Dir fonts = new();
    public Dir images = new();
    public Dir scripts = new();
    public Dir user = new();
    public Dir screens = new();
    public Dir libs = new();
    public Dir? project;

    public string luaExt = ".lua";
    public string mainFile = "main";

    public Directories()
    {
        // Set up paths relative to each other. TODO: Break this out into its own functions so user can set folder locations.
        resources.folder = "resources\\";
        resources.fullpath = resources.folder;

        fonts.folder = "fonts\\";
        fonts.fullpath = resources.fullpath + fonts.folder;

        images.folder = "images\\";
        images.fullpath = resources.fullpath + images.folder;

        scripts.folder = "scripts\\";
        scripts.fullpath = resources.fullpath + scripts.folder;

        user.folder = "user\\";
        user.fullpath = resources.fullpath + user.folder;

        screens.folder = "screens\\";
        screens.fullpath = user.fullpath + screens.folder;

        libs.folder = "libs\\";
        libs.fullpath = scripts.fullpath + libs.folder;

        // TODO: Make this dynamic
        project = new();
        project.folder = "test\\";
        project.fullpath = screens.fullpath + project.folder;
    }

    public List<String> GetProjects()
    {
        List<string> result = new();

        try
        {
            var dirs = Directory.GetDirectories(screens.fullpath);

            if (dirs.Length > 0)
            {
                foreach (string dir in dirs)
                {
                    if (IsFolderProject(dir))
                    {
                        string dirFin = dir.Split('\\').Last();
                        result.Add(dirFin);
                    }
                }
            }
        }
        catch (Exception e)
        {

        }

        return result;
    }

    // Accepts a full pathname.
    public bool IsFolderProject(string filename)
    {
        return (File.Exists(filename + "\\main.lua"));
    }
}
