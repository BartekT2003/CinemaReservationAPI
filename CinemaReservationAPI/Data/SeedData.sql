-- Add more movies
INSERT INTO Movies (Title, Description, DurationMinutes, Genre, ReleaseDate, PosterImagePath)
VALUES 
('The Dark Knight', 'When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.', 152, 'Action', '2008-07-18', '/posters/dark-knight.jpg'),
('Inception', 'A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.', 148, 'Sci-Fi', '2010-07-16', '/posters/inception.jpg'),
('The Shawshank Redemption', 'Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.', 142, 'Drama', '1994-09-23', '/posters/shawshank.jpg'),
('Pulp Fiction', 'The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.', 154, 'Crime', '1994-10-14', '/posters/pulp-fiction.jpg'),
('The Matrix', 'A computer programmer discovers that reality as he knows it is a simulation created by machines, and joins a rebellion to break free.', 136, 'Sci-Fi', '1999-03-31', '/posters/matrix.jpg'),
('Forrest Gump', 'The presidencies of Kennedy and Johnson, the Vietnam War, the Watergate scandal and other historical events unfold from the perspective of an Alabama man with an IQ of 75.', 142, 'Drama', '1994-07-06', '/posters/forrest-gump.jpg'),
('Interstellar', 'A team of explorers travel through a wormhole in space in an attempt to ensure humanity''s survival.', 169, 'Sci-Fi', '2014-11-07', '/posters/interstellar.jpg'),
('The Godfather', 'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.', 175, 'Crime', '1972-03-24', '/posters/godfather.jpg'),
('Jurassic Park', 'A pragmatic paleontologist visiting an almost complete theme park is tasked with protecting a couple of kids after a power failure causes the park''s cloned dinosaurs to run loose.', 127, 'Adventure', '1993-06-11', '/posters/jurassic-park.jpg'),
('Avatar', 'A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.', 162, 'Sci-Fi', '2009-12-18', '/posters/avatar.jpg');

-- Add more screenings (assuming theater IDs 1-3 exist)
-- Today's date for reference
DECLARE @Today DATE = GETDATE();

-- Add screenings for each movie over the next 7 days
INSERT INTO Screenings (MovieId, TheaterId, StartTime)
SELECT 
    m.Id as MovieId,
    t.Id as TheaterId,
    DATEADD(MINUTE, 
        CASE 
            WHEN ROW_NUMBER() OVER (PARTITION BY m.Id ORDER BY t.Id) = 1 THEN 0
            WHEN ROW_NUMBER() OVER (PARTITION BY m.Id ORDER BY t.Id) = 2 THEN 180
            ELSE 360
        END,
        DATEADD(DAY, number, @Today)
    ) as StartTime
FROM Movies m
CROSS JOIN (SELECT TOP 7 ROW_NUMBER() OVER (ORDER BY object_id) - 1 as number FROM sys.objects) as Numbers
CROSS JOIN (SELECT Id FROM Theaters WHERE Id IN (1, 2, 3)) as t
WHERE m.Id > (SELECT MAX(Id) - 10 FROM Movies) -- Only for the 10 new movies
ORDER BY m.Id, StartTime;

-- Update some screenings to be at different times of day
UPDATE Screenings
SET StartTime = DATEADD(HOUR, 
    CASE (ROW_NUMBER() OVER (PARTITION BY MovieId, CAST(StartTime AS DATE) ORDER BY StartTime))
        WHEN 1 THEN 14 -- 2 PM
        WHEN 2 THEN 17 -- 5 PM
        WHEN 3 THEN 20 -- 8 PM
        ELSE 22 -- 10 PM
    END,
    CAST(StartTime AS DATE))
WHERE MovieId IN (SELECT Id FROM Movies WHERE Id > (SELECT MAX(Id) - 10 FROM Movies)); 