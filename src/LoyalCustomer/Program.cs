﻿using static System.Console;

// Assume we have a website and we keep track of what pages customers are viewing.
//
// Every time somebody comes to the website, we write a record to a log file consisting of Timestamp, PageId, and
// CustomerId. At the end of each day we have a big log file with many entries in that format. And for every day we
// have a new file.  Each file is in CSV format, with the following format:
// 
// Timestamp,PageId,UserId
//
// where PageId and UserId are unique identifiers for the page accessed and the user who accessed the page.
//
// Now, given two log files (log file from day 1 and log file from day 2) we want to generate a list of 'loyal customers'
// which meet the criteria of: (a) they came on both days, and (b) they visited at least two unique pages.

public class Program
{
    private class LogRecord
    {
        public DateTime Timestamp { get; set; }
        public Guid PageId { get; set; }
        public Guid UserId { get; set; }
    }

    private static IList<Guid> GetLoyalUsers( IList<IEnumerable<LogRecord>> contentOfLogFiles )
    {
        var content = GetContentOfLogFiles( );
        var allUsers = content.SelectMany( (f, i) => f.Select( r => ( i, r ) ) ).GroupBy( ir => ir.r.UserId );

        var loyalUsers = allUsers.Where( grp => grp.Select( g => g.i ).Distinct( ).Count( ) > 1 ).ToList( );

        return loyalUsers.Select( grp => grp.Key ).ToList( );
    }

    private static IList<IEnumerable<LogRecord>> GetContentOfLogFiles( )
    {
        var list = new List<IEnumerable<LogRecord>>( );

        var funcSplit = new Func<string, IEnumerable<(DateTime dt, Guid pageId, Guid userId )>>( str => {
            // Right here the interviewer asked me to use a StringReader instead of doing a str.Split( '\n' ) that would take longer, but OK
            //var result = str.Split( '\n' )
            var result = GetLines( str )
            .Select( line => line?.Split( ',' ) )
            .Where( line => line is not null && line.Length == 3 )
            .Select( arr =>
            {
                var date = DateTime.Parse( arr![0], null, System.Globalization.DateTimeStyles.AdjustToUniversal );
                return (date, new Guid( arr[1] ), new Guid( arr[2] ) );
            } );
            return result;
        } );

        // Job interview test so I broke it out. Sometimes the next thing asked would be to modify and this provided
        // layers while also being encapsulated.
        list.Add(
            funcSplit( LogFileDay1Contents )
            .Select( f => new LogRecord( )
            {
                Timestamp = f.dt,
                PageId = f.pageId,
                UserId = f.userId
            } )
        );
        list.Add(
            funcSplit( LogFileDay2Contents )
            .Select( f => new LogRecord( )
            {
                Timestamp = f.dt,
                PageId = f.pageId,
                UserId = f.userId
            } )
        );

        return list;
    }

    static void Main( )
    {
        IList<IEnumerable<LogRecord>> contentOfLogFiles = GetContentOfLogFiles();

        IList<Guid> loyalUsers = GetLoyalUsers(contentOfLogFiles);
        foreach ( var loyalUser in loyalUsers )
        {
            Console.WriteLine( $"Loyal User: {loyalUser}" );
        }
    }

    private static IEnumerable<string> GetLines( string str )
    {
        string? line = null;
        using var sr = new StringReader( str );
        do
        {
            line = sr.ReadLine( );
            if ( line is not null )
            {
                yield return line;
            }
        }
        while ( line is not null );
    }

    // In memory simulation of log file contents for 2 days.

    private const string LogFileDay1Contents = @"2024-03-26T19:49:36.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,8a2561c4-ee85-4929-a28d-418c9bc7a55b
2024-03-26T19:49:37.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,64c00578-ffce-4678-8465-8a3f6faec8b4
2024-03-26T19:49:38.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,03e3d7a7-436e-4122-96a6-27d87a9b980d
2024-03-26T19:49:39.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,d169a7a5-b3ee-4eb2-9107-0b83ab001db2
2024-03-26T19:49:40.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,fd826e47-ece0-4a50-82b0-c758401212f7
2024-03-26T19:49:41.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,70a53ee1-369b-4c60-a046-5106cd865735
2024-03-26T19:49:42.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,13fecf65-2d29-4088-a5a0-cc671b4b9e33
2024-03-26T19:49:43.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,787f7f11-3f9f-4d40-8f7c-5f02205d63d3
2024-03-26T19:49:44.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,aa3b5214-a3f7-4e7e-a0a6-c061edf61f90
2024-03-26T19:49:45.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,e1df4cc3-e484-4616-96b4-8150c1a5203b
2024-03-26T19:49:46.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,4e9cf7fb-0843-497a-8176-9c59b7fe3d4c
2024-03-26T19:49:47.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,f66d7a4d-bd68-4f26-915a-9130a4b1848e
2024-03-26T19:49:48.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,66d96a82-3424-412d-acbb-c8e6ab69b4f7
2024-03-26T19:49:49.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,7bd016b7-a680-4b5f-aad2-4ccb93cdf350
2024-03-26T19:49:50.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,87d02e88-91c4-48cd-b68b-72fcc699f065
2024-03-26T19:49:51.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,3082fe9d-505c-42ac-a94a-5a3bd1af8d7b
2024-03-26T19:49:52.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,7d229bf4-dc04-48f2-be09-39de90493383
2024-03-26T19:49:53.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,ff4ee7d9-90a4-45ce-a7cc-fa1d81e12ae8
2024-03-26T19:49:54.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,596f4979-757b-4689-a985-7eecd1b039f4
2024-03-26T19:49:55.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,f2ce9c26-ffc3-4549-8c6a-a931e9000984
2024-03-26T19:49:56.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,9a256736-b9ac-406d-bcb7-f8634c6f8367
2024-03-26T19:49:57.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,1494fd5a-0ae2-49f2-b676-b243735edb9c
2024-03-26T19:49:58.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,bab614b2-53d0-4fe7-998b-c207a0b550ab
2024-03-26T19:49:59.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,d94fd4ee-8006-400b-a8a3-7455cd172a64
2024-03-26T19:50:00.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,f94b0330-f87a-47a2-88cf-eb3130539c2a
2024-03-26T19:50:01.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,52c8e869-4d08-4a37-8e92-d2116ccdca4d
2024-03-26T19:50:02.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,4da98791-2a5a-40ee-a7d5-2abef81ff2ff
2024-03-26T19:50:03.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,ecda1af7-bb2f-41d1-b3c8-490c6c8080c3
2024-03-26T19:50:04.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,63ff3139-c191-4e9c-988e-356e151795e0
2024-03-26T19:50:05.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,4da1fcd3-6df5-4bc0-b05c-e8e79b94ec7c
2024-03-26T19:50:06.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,fd826e47-ece0-4a50-82b0-c758401212f7
2024-03-26T19:50:07.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,4a155410-22a1-4f34-9df3-dc9742dc5577
2024-03-26T19:50:08.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,aa44a42d-a5e9-4b4b-9d61-a3b26f421fb6
2024-03-26T19:50:09.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,c03ddac7-0101-4148-a573-639f7bc439df
2024-03-26T19:50:10.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,b4ee0640-970f-4494-a723-9baa6119a968
2024-03-26T19:50:11.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,390ca661-ca48-4be0-8330-be25c38245c4
2024-03-26T19:50:12.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,3e888c8e-706a-4f6d-bba7-be29a7116772
2024-03-26T19:50:13.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,9bae324f-ff78-4754-bbb0-82fea908ade2
2024-03-26T19:50:14.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,f0a220e6-40bb-4d6b-86be-4ce059f5d9da
2024-03-26T19:50:15.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,1af97edc-56dd-4052-94f7-dcf8996c6ab9
2024-03-26T19:50:16.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,d169a7a5-b3ee-4eb2-9107-0b83ab001db2
2024-03-26T19:50:17.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,f4575bd3-5184-4c74-9764-293916df9b8b
2024-03-26T19:50:18.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,10e45f64-a401-4bda-aec8-828beafbbced
2024-03-26T19:50:19.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,3643c99e-c940-49de-a7df-bfdfb92cac99
2024-03-26T19:50:20.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,1ea5567f-d898-4b59-8a46-6235a372e0f2
2024-03-26T19:50:21.8532130Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,f29c6406-8288-49de-9da0-2b3f3063df50
2024-03-26T19:50:22.8532130Z,5fef20b1-5ce5-4369-94a0-254958193155,7886a74f-7b3a-4acf-a80f-ec20c1aa2fa7
2024-03-26T19:50:23.8532130Z,0f775f3d-f295-4376-baf8-a8ce42e2d08e,7dd46e9c-6bf0-45a1-84a0-54f4edda7fe4
2024-03-26T19:50:24.8532130Z,f07227b5-7afe-4c0e-ad7a-99a80d54b678,f5ceb53e-8fed-46af-b7b9-b6c45d6ba296
2024-03-26T19:50:25.8532130Z,8e3824fe-08b0-46cd-915c-8262ecf0f057,e596871b-762d-4b9d-ae42-5a4369adec99
";

    private const string LogFileDay2Contents = @"2024-03-27T19:47:20.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,cd5730d0-48de-4235-9052-8d4bfe1cc818
2024-03-27T19:47:21.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,fcd4fac8-650f-411b-8ac7-c30cc6506b50
2024-03-27T19:47:22.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,e1433f83-73e6-46af-a8b2-4c321da63430
2024-03-27T19:47:23.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,633077a7-d9f8-4eef-8da5-27b722e12993
2024-03-27T19:47:24.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,c1060722-ff49-4a83-8fb6-3e3165c36cb7
2024-03-27T19:47:25.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,aa6f94e6-9d7e-4766-89cf-d675559fffee
2024-03-27T19:47:26.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,3b30441f-255c-45e5-9113-b2a03e9a6a24
2024-03-27T19:47:27.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,6022bf47-58ea-4d22-b401-25943049f60f
2024-03-27T19:47:28.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,fe313091-6188-46ee-9665-fe61fb733b7c
2024-03-27T19:47:29.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,30564c61-369b-43b4-9ae3-ed4ba052c01a
2024-03-27T19:47:30.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,c91ff259-2955-4ca1-85ec-dac3fbfe2653
2024-03-27T19:47:31.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,e6eb522d-233e-471b-9f67-1ef09d00c30f
2024-03-27T19:47:32.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,d39bc494-5e7e-4880-9937-a5d2ab385455
2024-03-27T19:47:33.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,4e37b3f3-f73f-4fe0-be6c-c5bb02a93653
2024-03-27T19:47:34.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,272241ed-47fd-4231-ad77-adc8dd134ff0
2024-03-27T19:47:35.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,d47e29e1-f918-4221-bf42-ce320d60af17
2024-03-27T19:47:36.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,a7ad2b5f-e4af-432d-9a94-115c0a6caec2
2024-03-27T19:47:37.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,6cbc9ea5-558b-40bf-a66a-c63c71333ec2
2024-03-27T19:47:38.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,e6d38306-eafc-4c5c-a708-80800c91519c
2024-03-27T19:47:39.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,a58e91a5-0fff-46a0-9722-eec91655404e
2024-03-27T19:47:40.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,18ced5c9-8cd1-490e-a2ee-00f517b53bb9
2024-03-27T19:47:41.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,5349c44a-0108-40c8-90de-f3b1c1ffe78c
2024-03-27T19:47:42.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,e2b282fb-28aa-4746-8733-24c4acebec62
2024-03-27T19:47:43.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,6e0c10d2-448d-4e6b-92f4-e2ada9452d24
2024-03-27T19:47:44.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,d14f3abd-30de-4fa4-9ba5-4278e698ac98
2024-03-27T19:47:45.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,c06881bc-4db7-4cf7-8cb2-13f559516321
2024-03-27T19:47:46.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,5825ea42-5e2c-4a11-916c-233cc808ea03
2024-03-27T19:47:47.4900470Z,f2f32ee5-3c2e-4135-915a-1c89c227ad6f,fd826e47-ece0-4a50-82b0-c758401212f7
2024-03-27T19:47:48.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,89211072-c0a7-4059-9a97-5d71224e9b59
2024-03-27T19:47:49.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,820ee8b5-a938-4855-a2c8-95d103056d78
2024-03-27T19:47:50.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,368fcdf3-874e-48ff-8826-19e196cbb00c
2024-03-27T19:47:51.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,7752d7aa-5f14-480d-91b3-a748b69fef4a
2024-03-27T19:47:52.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,0d2cb545-2509-4425-be44-807d6a407baf
2024-03-27T19:47:53.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,50194fda-cec3-4573-bd8a-2f82f06d4348
2024-03-27T19:47:54.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,f44a1b48-cd2d-42f2-bc15-8e3a9b0fda1a
2024-03-27T19:47:55.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,9bffdaf3-bfa6-4405-b6b6-d23cea54e4ed
2024-03-27T19:47:56.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,12a8c506-56c8-413e-8d33-b37bfe55d250
2024-03-27T19:47:57.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,f6a8ca7c-1540-4bc4-8f1c-13522ccdc642
2024-03-27T19:47:58.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,dc059ce3-acdf-4dd5-b854-a55c43b38c06
2024-03-27T19:47:59.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,671f72c1-8e74-4fe4-ad29-f754e10fcac5
2024-03-27T19:48:00.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,09a52c2b-68a3-479a-a9e2-416fdfa333f1
2024-03-27T19:48:01.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,f550961b-ded2-4d1f-9776-f2b39a64fc00
2024-03-27T19:48:02.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,86f526a7-a415-4733-9cdc-2db2817fc414
2024-03-27T19:48:03.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,3a33860c-104d-40c0-9c66-9773116283eb
2024-03-27T19:48:04.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,aa3b5214-a3f7-4e7e-a0a6-c061edf61f90
2024-03-27T19:48:05.4900470Z,6ef7c385-17a5-4616-a9e0-7def535b640b,b25e9ed3-ace8-47de-9417-6f5bbb1e6647
2024-03-27T19:48:06.4900470Z,3b6d7121-6429-4598-8960-5cc6f1e906f2,6ebbd841-427a-4054-91a8-2aa90be495ae
2024-03-27T19:48:07.4900470Z,842ffcc3-b4db-42ce-a638-16baf282401a,f8c5c315-1a21-464f-a6b3-40d7ee883ae2
2024-03-27T19:48:08.4900470Z,66c890c2-3185-4ec8-b874-1cad54ec5f36,8fd8c29c-182a-4a52-a077-632758583bac
2024-03-27T19:48:09.4900470Z,58a7b9ae-443d-446b-96d5-49068648bba6,2e67c64e-2af9-4e73-9d8f-995caa0c65d8
";
}


