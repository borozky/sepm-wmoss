using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Entities;
using WMoSS.Repositories;
using WMoSS.Services;

namespace WMoSS.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IExpressBookingDataProvider _expressBookingDataProvider;

        public IndexModel(
            IMovieRepository movieRepository,
            IExpressBookingDataProvider expressBookingDataProvider)
        {
            _movieRepository = movieRepository;
            _expressBookingDataProvider = expressBookingDataProvider;
        }

        public IEnumerable<FeatureSlide> FeatureSlides { get; set; }
        public IEnumerable<Movie> Movies { get; set; }

        public void OnGet()
        {
            FeatureSlides = new FeatureSlide[]
            {
                new FeatureSlide
                {
                    Title = "Lorem ipsum",
                    Subtitle = "Welcome to our website",
                    CallToActionUrl = "#",
                    BackgroundImageUrl = "http://placehold.it/960x410/ab9bff/fff"
                },
                new FeatureSlide
                {
                    Title = "Lorem ipsum",
                    Subtitle = "Welcome to our website",
                    CallToActionUrl = "#",
                    BackgroundImageUrl = "http://placehold.it/960x410/ffee9b/444"
                },
            };
        }
        
    }
    
    public class FeatureSlide
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string CallToActionUrl { get; set; }
        public string BackgroundImageUrl { get; set; }
    }
}
