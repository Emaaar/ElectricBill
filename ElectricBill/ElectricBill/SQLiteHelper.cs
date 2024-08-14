using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElectricBill
{
   public class SQLiteHelper
    {

        
            SQLiteAsyncConnection db;
            public SQLiteHelper(string dbPath)
            {
                db = new SQLiteAsyncConnection(dbPath);
                db.CreateTableAsync<Reading>().Wait();
            }

            //Insert and Update new record  
            public Task<int> SaveItemAsync(Reading person)
            {
                if (person.MeterNumber != 0)
                {
                    return db.UpdateAsync(person);
                }
                else
                {
                    return db.InsertAsync(person);
                }
            }

            //Delete  
            public Task<int> DeleteItemAsync(Reading person)
            {
                return db.DeleteAsync(person);
            }

            //Read All Items  
            public Task<List<Reading>> GetItemsAsync()
            {
                return db.Table<Reading>().ToListAsync();
            }


            //Read Item  
            public Task<Reading> GetItemAsync(int personId)
            {
                return db.Table<Reading>().Where(i => i.MeterNumber == personId).FirstOrDefaultAsync();
            }
        }
    }



