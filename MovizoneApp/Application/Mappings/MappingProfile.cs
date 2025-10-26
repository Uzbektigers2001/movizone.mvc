using AutoMapper;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between Domain models and DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Movie mappings
            CreateMap<Movie, MovieDto>();
            CreateMap<MovieDto, Movie>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            CreateMap<CreateMovieDto, Movie>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate ?? DateTime.UtcNow));
            CreateMap<UpdateMovieDto, Movie>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate ?? DateTime.UtcNow));
            // Reverse mappings for AdminController
            CreateMap<Movie, CreateMovieDto>();
            CreateMap<Movie, UpdateMovieDto>();

            // TVSeries mappings
            CreateMap<TVSeries, TVSeriesDto>()
                .ForMember(dest => dest.TotalSeasons, opt => opt.MapFrom(src => src.Seasons))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Creator));
            CreateMap<TVSeriesDto, TVSeries>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Seasons, opt => opt.MapFrom(src => src.TotalSeasons))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Director));
            CreateMap<CreateTVSeriesDto, TVSeries>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Episodes, opt => opt.Ignore())
                .ForMember(dest => dest.ShowInBanner, opt => opt.Ignore())
                .ForMember(dest => dest.FirstAired, opt => opt.Ignore())
                .ForMember(dest => dest.Seasons, opt => opt.MapFrom(src => src.TotalSeasons))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Director));
            CreateMap<UpdateTVSeriesDto, TVSeries>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Episodes, opt => opt.Ignore())
                .ForMember(dest => dest.ShowInBanner, opt => opt.Ignore())
                .ForMember(dest => dest.FirstAired, opt => opt.Ignore())
                .ForMember(dest => dest.Seasons, opt => opt.MapFrom(src => src.TotalSeasons))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Director));
            // Reverse mappings for AdminController
            CreateMap<TVSeries, CreateTVSeriesDto>()
                .ForMember(dest => dest.TotalSeasons, opt => opt.MapFrom(src => src.Seasons))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Creator));
            CreateMap<TVSeries, UpdateTVSeriesDto>()
                .ForMember(dest => dest.TotalSeasons, opt => opt.MapFrom(src => src.Seasons))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Creator));

            // Actor mappings
            CreateMap<Actor, ActorDto>();
            CreateMap<ActorDto, Actor>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            CreateMap<CreateActorDto, Actor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate ?? DateTime.UtcNow.AddYears(-30)));
            CreateMap<UpdateActorDto, Actor>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate ?? DateTime.UtcNow.AddYears(-30)));
            // Reverse mappings for AdminController
            CreateMap<Actor, CreateActorDto>();
            CreateMap<Actor, UpdateActorDto>();

            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Don't map password
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Avatar, opt => opt.Ignore());
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // Password will be hashed separately
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            // Reverse mappings for AdminController
            CreateMap<User, UpdateUserDto>();

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.MovieTitle, opt => opt.Ignore()) // Will be populated from Movie
                .ForMember(dest => dest.UserName, opt => opt.Ignore()); // Will be populated from User
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
            CreateMap<UpdateReviewDto, Review>()
                .ForMember(dest => dest.MovieId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

            // Watchlist mappings
            CreateMap<WatchlistItem, WatchlistDto>()
                .ForMember(dest => dest.MovieTitle, opt => opt.Ignore()) // Will be populated from Movie
                .ForMember(dest => dest.MovieCoverImage, opt => opt.Ignore())
                .ForMember(dest => dest.MovieGenre, opt => opt.Ignore())
                .ForMember(dest => dest.MovieRating, opt => opt.Ignore());
            CreateMap<WatchlistDto, WatchlistItem>();
            CreateMap<CreateWatchlistItemDto, WatchlistItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

            // PricingPlan mappings
            CreateMap<PricingPlan, PricingPlanDto>();
            CreateMap<CreatePricingPlanDto, PricingPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
            CreateMap<UpdatePricingPlanDto, PricingPlan>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
        }
    }
}
