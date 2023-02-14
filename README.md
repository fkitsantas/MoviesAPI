# Movies API

This API is built using C# ASP.NET Core with Entity Framework Core and MSSQL.
The solution uses a T-SQL stored procedure to calculate the average rating of a movie and update the corresponding value in the `Movies` table when a new rating is inserted into the `Ratings` table.

## Create the MoviesAPI MSSQL Database

The following code creates the necessary tables for the API:

```sql
USE MoviesAPI;

-- Create the Movies table to store movie information
CREATE TABLE Movies (
    movie_id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(255) NOT NULL,
    yearOfRelease INT NOT NULL,
    genre NVARCHAR(255) NOT NULL,
    rating FLOAT NULL
);

-- Create the Users table to store user information
CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    firstName NVARCHAR(255) NOT NULL,
    lastName NVARCHAR(255) NOT NULL
);

-- Create the Ratings table to store movie ratings by users
CREATE TABLE Ratings (
    user_id INT NOT NULL,
    movie_id INT NOT NULL,
    rating REAL NOT NULL,
    PRIMARY KEY (user_id, movie_id),
    FOREIGN KEY (user_id) REFERENCES Users(user_id), -- Link to the user_id in the Users table
    FOREIGN KEY (movie_id) REFERENCES Movies(movie_id) -- Link to the movie_id in the Movies table
);

-- Insert sample data into the Movies table
INSERT INTO Movies (title, yearOfRelease, genre)
VALUES ('Great Expectations', 1998, 'Drama'), 
       ('Hackers', 1995, 'Thriller'), 
       ('Johnny Mnemonic', 1995, 'Sci-Fi');

-- Insert sample data into the Users table
INSERT INTO Users (firstName, lastName)
VALUES ('Fotis', 'Kitsantas'), 
       ('Reina', 'Rapi'),
       ('Spiros', 'Zamprakos');

-- Insert sample data into the Ratings table
INSERT INTO Ratings (user_id, movie_id, rating)
VALUES  (1, 1, 5.0),
        (1, 2, 4.5),
        (2, 2, 5.0),
        (3, 2, 3.0),
        (3, 1, 4.0);
```

## proc_add_or_update_movie_rating T-SQL Procedure

The following T-SQL procedure is used to add or update a rating for a movie by a specific user:

```sql
CREATE PROCEDURE proc_add_or_update_movie_rating
    @user_id INT,
    @movie_id INT,
    @rating REAL
AS
BEGIN
    -- Check if the user and movie exist
    DECLARE @user_exists INT = (SELECT COUNT(*) FROM Users WHERE user_id = @user_id);
    DECLARE @movie_exists INT = (SELECT COUNT(*) FROM Movies WHERE movie_id = @movie_id);

    IF @user_exists = 0 OR @movie_exists = 0
    BEGIN
        RETURN 404;
    END;

    -- Check if the rating is valid
    IF @rating < 0 OR @rating > 5
    BEGIN
        RETURN 400;
    END;

    -- Check if the rating already exists
    DECLARE @rating_exists INT = (SELECT COUNT(*) FROM Ratings WHERE user_id = @user_id AND movie_id = @movie_id);

    IF @rating_exists = 0
    BEGIN
        -- Insert a new rating
        INSERT INTO Ratings (user_id, movie_id, rating)
        VALUES (@user_id, @movie_id, @rating);
    END;
    ELSE
    BEGIN
        -- Update the existing rating
        UPDATE Ratings
        SET rating = @rating
        WHERE user_id = @user_id AND movie_id = @movie_id;
    END;

    -- Update the average rating for the movie
    DECLARE @result FLOAT;
    SET @result = (SELECT AVG(rating) FROM Ratings WHERE movie_id = @movie_id);
    UPDATE Movies
    SET rating = ROUND(@result * 2.0, 0) / 2.0
    WHERE movie_id = @movie_id;

    RETURN 200;
END;
```



---



# Appendix:

This appendix provides the T-SQL code of my initial attempt to calculate the average rating of a movie and update the corresponding value in the Movies table. My initial approach involved creating a function and a trigger in T-SQL, with the function used to calculate the average rating and the trigger utilized to update the Movies table when a new rating was inserted into the Ratings table. However, after further consideration, it was determined that this approach was not efficient and a single stored procedure was implemented as a more suitable solution. The code for the function and trigger is presented here as a demonstration of the exploratory work that was performed.

## avg_movie_rating T-SQL Function

The following T-SQL function was written to calculate the average rating of a movie, rounded to the nearest half value.

```sql
CREATE FUNCTION avg_movie_rating (@movie_id INT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @result FLOAT;
    SET @result = (SELECT AVG(rating) FROM Ratings WHERE movie_id = @movie_id);
    RETURN ROUND(@result * 2.0, 0) / 2.0;
END;
```

## update_movie_rating T-SQL Trigger

The following T-SQL trigger was written to update the rating of a movie in the `Movies` table whenever a new rating is inserted into the `Ratings` table.

```sql
CREATE TRIGGER update_movie_rating
ON Ratings
AFTER INSERT
AS
BEGIN
    DECLARE @movie_id INT = (SELECT movie_id FROM INSERTED);
    UPDATE Movies
    SET rating = dbo.avg_movie_rating(@movie_id)
    WHERE movie_id = @movie_id;
END;
```