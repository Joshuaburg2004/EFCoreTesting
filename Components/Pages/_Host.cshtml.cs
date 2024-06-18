// Required for accepting post data from 3rd party resource.
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

[IgnoreAntiforgeryToken]
// Not required, but gives us access to the post data and exception handling per request.
public class HostPageModel : PageModel
{
    /// <summary>
    /// A list of components we accept post request on.
    /// </summary>
    public static List<string> PostComponentPages = new List<string>() { "/counter" };

    // postFormService is injected by the DI
    public HostPageModel(PostFormService postFormService)
    {
        PostFormService = postFormService;
    }

    private PostFormService PostFormService { get; }

    // Hook in to the OnPost.
    public void OnPost()
    {
        if (PostComponentPages.Any(c => Request.Path.ToString().Contains(c)))
            PostFormService.Form = Request.Form; // acceptable component, store the post form in the PostFormService
        else
            throw new Exception("HTTP 401 Error – Unauthorized");
    }

}