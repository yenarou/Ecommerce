const fs = require('fs')
const path = require('path')
const Database = require('better-sqlite3')

const DB_PATH = path.join(__dirname, 'data.sqlite')
const SQL_PATH = path.join(__dirname, 'db.sql')

if (fs.existsSync(DB_PATH)) {
  fs.unlinkSync(DB_PATH)
  console.log('Base de datos anterior eliminada.')
}

const db = new Database(DB_PATH)
db.pragma('foreign_keys = ON')

const sql = fs.readFileSync(SQL_PATH, 'utf-8')
db.exec(sql)

db.close()
console.log('Esquema y datos semilla aplicados desde db.sql.')
console.log(`Listo. Base de datos en: ${DB_PATH}`)
