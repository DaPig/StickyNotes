
-------- tables for database-------------------

CREATE TABLE IF NOT EXISTS category (
  type char(20) NOT NULL,
  PRIMARY KEY (type)
);

-- --------------------------------------------------------

--
-- Table structure for table hasCategory
--

CREATE TABLE IF NOT EXISTS hasCategory (
  note int(11) NOT NULL,
  category char(20) DEFAULT NULL,
  PRIMARY KEY (note)
);

-- --------------------------------------------------------

--
-- Table structure for table note
--

CREATE TABLE IF NOT EXISTS note (
  id int(11) NOT NULL AUTO_INCREMENT,
  content text NOT NULL,
  user char(7) DEFAULT NULL,
  PRIMARY KEY (id)
);

-- --------------------------------------------------------

--
-- Table structure for table user
--

CREATE TABLE IF NOT EXISTS user (
  id char(7) NOT NULL,
  name text NOT NULL,
  PRIMARY KEY (id)
);