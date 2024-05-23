Readme
======
Original Description:

Assume we have a website and we keep track of what pages customers are viewing.

Every time somebody comes to the website, we write a record to a log file consisting of Timestamp, PageId, and
CustomerId. At the end of each day we have a big log file with many entries in that format. And for every day we
have a new file.  Each file is in CSV format, with the following format:
 
Timestamp,PageId,UserId

where PageId and UserId are unique identifiers for the page accessed and the user who accessed the page.

Now, given two log files (log file from day 1 and log file from day 2) we want to generate a list of 'loyal customers'
which meet the criteria of: (a) they came on both days, and (b) they visited at least two unique pages.


This project contains two program mains. One has been removed from compile.
The first version is the version I did in the interview while also chatting about the actual code.

The program_optimized is the one I did just for fun later when I gave some thought to the issue. I can understand
the interviewers didn't like my first version because it was verbose. However, it's my style in interviews
since you know that changes could get asked for that are obviously going to be tricky to solve if you
prematurely optimized by design._
