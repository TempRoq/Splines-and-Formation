# Splines-and-Formation
Spline path creator and following algorithm that supports formations of objects 

===SUMMARY===

I needed data structure and algorithm that allowed for enemies in different formations to follow a path as seen in shmups like galaga through enemies and touhou through bullets.

This data structure needed to
> Store a path as concisely as possible
> Allow for smooth paths to be created
> Allow for variations of paths to be made as easily as possible

THe algorithm needed to allow objects to follow a path
> by itself
> around a moving origin
> at an offset
  > Speed
  > Time
  > Position
  > Angle
  > Scale

I realized very early on that storing a new path for every object that was supposed to follow a path would be ridiculous. Editing levels would be pain if 10 ships in a formation need to have their paths slightly altered. This does not even take into account the amount of data this would use, nor the time it would take to actually set up individual paths for hundreds of enemies and bullets. 

I eventually came to the idea of using Bezier Curves, however instead of using the usual bezier curve algorithms that operate by teleporting the object between different points in an update function, I could have an object find its next point and find the velocity. The velocity could then be modified to account for offsets. This allows for potentially dozens of objects to use a single path. 



===Taken from my thought process document===

Attempt 1
  Goal: To make a way to let enemies follow a given path
  Solution: Basic steering and using a series of structs that carry data on how to modify the speed, acceleration, angle, angular velocity, etc
    Did it work?
    yeah… but…
    Problem:
      Too much information for a very basic path
      Too much stuff to track
      Too much math to make a path look believable
      Although, it was pretty good in small doses and the logic could be implemented into an AI steering, so I kept the code for it.
Attempt 2
  Goal: Attempt 1, but with less data to store and track and less math
  Solution: BEZIER CURVES ARE AMAZING. Use bezier curves for basic positional following and to make it so some paths can parent other paths.
    Did it work?
    yeah… but…
    Problem:
      Making enemies follow a path with some slight variations was nasty, as either it would
      Be too slow at runtime by attempting to do math to find new points for every enemy every frame, rendering it very unscalable
      Be too much work on a designer when trying to make slight variations on paths.
      Was on the right path, however not fully there yet
Attempt 3
  Goal: Attempt 2, but with easier variations on a path
  Solution: Instead of tracking positions in space, track velocity by finding points on the path, and doing variations of the velocity vector
    Did it work?
    YES…
      Scaling was as easy as scaling the resultant velocity vector by another vector, which was very simple
      Finding an offset of a path and maintaining a formation was INFINITELY easier
      Knowing the velocity means knowing the forward vector, so normalizing it and finding the normal vector gave you a new right vector, using this, you could
      add offsets based on the new forward and right vectors and find the proper position to position velocity. For minimal extra math, you can scale a path and
      maintain formation.
      Rotating a vector was so much easier
      Save the magnitude of the vector, Normalize it, find ATan of the vector, convert it to degree measurement, add the angle, turn it into radians, use cos to 
      find the x component and sin to find the y component of the new direction, multiply it by the magnitude. It’s ugly, but efficient!
      Having a path dependent on another path was so much easier
      Just add the velocity of the parent to the independent velocity of the child and you will get the actual velocity of the child.
      This means that a whole fleet can use one path, which cuts down significantly on storage
      However…
        Cyclical dependencies in classes by allowing Positions to have sub formations made it impossible on the Unity Editor’s inspector side.
        Solution: Keep the data structure as is, make an external editor
