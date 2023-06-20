//using Core.Db.Ef;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//namespace Core.CommonModels.Enums
//{
//    public static class SeedData
//    {
//        public static void EnsurePopulated(IApplicationBuilder app)
//        {
//            ApplicationContext context = app.ApplicationServices
//                .CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();
//            if (context.Database.GetPendingMigrations().Any())
//            {
//                context.Database.Migrate();
//            }

//            if (!context.Tags.Any())
//            {
//                var newTags = new List<Tag>()
//                {
//                    new Tag
//                    {
//                        TagName = "1"
//                    },
//                    new Tag
//                    {
//                        TagName = "2"
//                    },
//                    new Tag
//                    {
//                        TagName = "3"
//                    },
//                    new Tag
//                    {
//                        TagName = "4"
//                    },


//                };
//                context.Tags.AddRange(newTags);
//                context.SaveChanges();
//            }

//            if (!context.Posts.Any())
//            {
//                var newPosts = new List<Post>()
//                {
//                    new Post
//                    {
//                        OwnerId = "c0d94f15-1589-49aa-90a9-5d0f475e63d6",
//                        Title = "Имя поста 1",
//                        Content = "Сам пост №1"
//                    },
//                    new Post
//                    {
//                        OwnerId = "c0d94f15-1589-49aa-90a9-5d0f475e63d6",
//                        Title = "Имя поста 1",
//                        Content = "Сам пост №1",
//                    },
//                    new Post
//                    {
//                        OwnerId = "c0d94f15-1589-49aa-90a9-5d0f475e63d6",
//                        Title = "Имя поста 1",
//                        Content = "Сам пост №1",
//                    },
//                    new Post
//                    {
//                        OwnerId = "c0d94f15-1589-49aa-90a9-5d0f475e63d6",
//                        Title = "Имя поста 1",
//                        Content = "Сам пост №1",
//                    }
//                };
//                var tags = context.Tags.ToList();
//                if (tags.Any())
//                {
//                    foreach (var post in newPosts)
//                    {
//                        var newTagsNav = tags.Select(x => new TagNavigation() { PostTagId = x.Id, PostId = post.Id });
//                        context.TagNavigation.AddRange(newTagsNav);
//                    }
//                }
//                context.Posts.AddRange(newPosts);
//                context.SaveChanges();

//            }
//        }
//    }
//}
