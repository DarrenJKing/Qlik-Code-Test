Readme
======

Assume we have a website and we keep track of what pages customers are viewing.

Every time somebody comes to the website, we write a record to a log file consisting of Timestamp, PageId, and
CustomerId. At the end of each day we have a big log file with many entries in that format. And for every day we
have a new file.  Each file is in CSV format, with the following format:
 
Timestamp,PageId,UserId

where PageId and UserId are unique identifiers for the page accessed and the user who accessed the page.

Now, given two log files (log file from day 1 and log file from day 2) we want to generate a list of 'loyal customers'
which meet the criteria of: (a) they came on both days, and (b) they visited at least two unique pages.
