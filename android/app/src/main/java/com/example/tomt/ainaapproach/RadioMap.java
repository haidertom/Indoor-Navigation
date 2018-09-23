package com.example.tomt.ainaapproach;

/**
 * Created by Tom on 01.12.17.
 */

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;


public class RadioMap {
    public String NaN = "-110";
    public File RadiomapMean_File = null;
    public ArrayList<String> MacAdressList = null;
    public HashMap<String, ArrayList<String>> LocationRSS_HashMap = null;
    public ArrayList<String> OrderList = null;

    public RadioMap(){
        NaN = "-110";
        RadiomapMean_File = null;
        MacAdressList = null;
        LocationRSS_HashMap = null;
        OrderList = null;
    }

    public RadioMap(File inFile) throws Exception {
        MacAdressList = new ArrayList<String>();
        LocationRSS_HashMap = new HashMap<String, ArrayList<String>>();
        OrderList = new ArrayList<String>();

        if (!ConstructRadioMap(inFile)) {
            throw new Exception("Inavlid Radiomap File");
        }
    }

    //added by Tom------------------------------------

    public RadioMap(String filePath) throws Exception {
        MacAdressList = new ArrayList<String>();
        LocationRSS_HashMap = new HashMap<String, ArrayList<String>>();
        OrderList = new ArrayList<String>();

        if (!ConstructRadioMap(filePath)) {
            throw new Exception("Inavlid Radiomap File");
        }
    }

    //------------------------------------------------


    /**
     * Getter of MAC Address list in file order
     *
     * @return the list of MAC Addresses
     * */
    public ArrayList<String> getMacAdressList() {
        return MacAdressList;
    }

    /**
     * Getter of HashMap Location-RSS Values list in no particular order
     *
     * @return the HashMap Location-RSS Values
     * */
    public HashMap<String, ArrayList<String>> getLocationRSS_HashMap() {
        return LocationRSS_HashMap;
    }

    /**
     * Getter of Location list in file order
     *
     * @return the Location list
     * */
    public ArrayList<String> getOrderList() {
        return OrderList;
    }

    /**
     * Getter of radio map mean filename
     *
     * @return the filename of radiomap mean used
     * */
    public File getRadiomapMean_File() {
        return this.RadiomapMean_File;
    }

    public String getNaN() {
        return NaN;
    }

    /**
     * Construct a radio map
     *
     * @param inFile
     *            the radio map file to read
     *
     * @return true if radio map constructed successfully, otherwise false
     * */
    private boolean ConstructRadioMap(File inFile) {

        if (!inFile.exists() || !inFile.canRead()) {
            return false;
        }

        this.RadiomapMean_File = inFile;
        this.OrderList.clear();
        this.MacAdressList.clear();
        this.LocationRSS_HashMap.clear();

        ArrayList<String> RSS_Values = null;
        BufferedReader reader = null;
        String line = null;
        String[] temp = null;
        String key = null;

        try {

            reader = new BufferedReader(new FileReader(inFile));

            // Read the first line # NaN -110
            line = reader.readLine();
            temp = line.split(" ");
            if (!temp[1].equals("NaN"))
                return false;
            NaN = temp[2];
            line = reader.readLine();

            // Must exists
            if (line == null)
                return false;

            line = line.replace(", ", " ");
            temp = line.split(" ");

            final int startOfRSS = 4;

            // Must have more than 4 fields
            if (temp.length < startOfRSS)
                return false;

            // Store all Mac Addresses Heading Added
            for (int i = startOfRSS; i < temp.length; ++i)
                this.MacAdressList.add(temp[i]);

            while ((line = reader.readLine()) != null) {

                if (line.trim().equals(""))
                    continue;

                line = line.replace(", ", " ");
                temp = line.split(" ");

                if (temp.length < startOfRSS)
                    return false;

                key = temp[0] + " " + temp[1];

                RSS_Values = new ArrayList<String>();

                for (int i = startOfRSS - 1; i < temp.length; ++i)
                    RSS_Values.add(temp[i]);

                // Equal number of MAC address and RSS Values
                if (this.MacAdressList.size() != RSS_Values.size())
                    return false;

                this.LocationRSS_HashMap.put(key, RSS_Values);

                this.OrderList.add(key);
            }

        } catch (Exception ex) {
            return false;
        } finally {
            if (reader != null)
                try {
                    reader.close();
                } catch (IOException e) {

                }
        }
        return true;
    }

    //added by Tom ------------------------------------

    private boolean ConstructRadioMap(String filePath) {

        this.RadiomapMean_File = new File(filePath);
        this.OrderList.clear();
        this.MacAdressList.clear();
        this.LocationRSS_HashMap.clear();

        ArrayList<String> RSS_Values = null;
        BufferedReader reader = null;
        String line = null;
        String[] temp = null;
        String key = null;

        try {

            reader = new BufferedReader(new FileReader(new File(filePath)));

            // Read the first line # NaN -110
            line = reader.readLine();
            temp = line.split(" ");
            if (!temp[1].equals("NaN"))
                return false;
            NaN = temp[2];
            line = reader.readLine();

            // Must exists
            if (line == null)
                return false;

            line = line.replace(", ", " ");
            temp = line.split(" ");

            final int startOfRSS = 4;

            // Must have more than 4 fields
            if (temp.length < startOfRSS)
                return false;

            // Store all Mac Addresses Heading Added
            for (int i = startOfRSS; i < temp.length; ++i)
                this.MacAdressList.add(temp[i]);

            while ((line = reader.readLine()) != null) {

                if (line.trim().equals(""))
                    continue;

                line = line.replace(", ", " ");
                temp = line.split(" ");

                if (temp.length < startOfRSS)
                    return false;

                key = temp[0] + " " + temp[1];

                RSS_Values = new ArrayList<String>();

                for (int i = startOfRSS - 1; i < temp.length; ++i)
                    RSS_Values.add(temp[i]);

                // Equal number of MAC address and RSS Values
                if (this.MacAdressList.size() != RSS_Values.size())
                    return false;

                this.LocationRSS_HashMap.put(key, RSS_Values);

                this.OrderList.add(key);
            }

        } catch (Exception ex) {
            return false;
        } finally {
            if (reader != null)
                try {
                    reader.close();
                } catch (IOException e) {

                }
        }
        return true;
    }

    //-------------------------------------------------

/*


    public static Collection<WeightedLatLng> readRadioMapLocations(File inFile) {
        class Weight {
            String lat;
            String lot;
            int intesity;
        }
        //sixth decimal place is worth up to 0.11 m
        final int decimal_place = 6;
        HashMap<String, Weight> locations = new HashMap<String, Weight>();
        BufferedReader reader = null;
        String line = null;
        String[] temp = null;
        String key = null;
        try {

            reader = new BufferedReader(new FileReader(inFile));

            // Read the first line # NaN -110
            line = reader.readLine();
            temp = line.split(" ");
            if (!temp[1].equals("NaN"))
                return null;

            line = reader.readLine();

            // Must exists
            if (line == null)
                return null;

            line = line.replace(", ", " ");
            temp = line.split(" ");

            final int startOfRSS = 4;

            // Must have more than 4 fields
            if (temp.length < startOfRSS)
                return null;

            while ((line = reader.readLine()) != null) {

                if (line.trim().equals(""))
                    continue;

                line = line.replace(", ", " ");
                temp = line.split(" ");

                if (temp.length < startOfRSS)
                    return null;

                String lat;
                String lot;
                try {
                    lat = temp[0].substring(0, temp[0].indexOf(".") + decimal_place);
                } catch (IndexOutOfBoundsException e) {
                    lat = temp[0];
                }

                try {
                    lot = temp[1].substring(0, temp[1].indexOf(".") + decimal_place);
                } catch (IndexOutOfBoundsException e) {
                    lot = temp[1];
                }

                key = lat + " " + lot;
                Weight weight = locations.get(key);
                if (weight == null) {
                    weight = new Weight();
                    weight.lat = temp[0];
                    weight.lot = temp[1];
                    locations.put(key, weight);
                }

                weight.intesity++;
            }

            Collection<WeightedLatLng> collection = new ArrayList<WeightedLatLng>();
            for (Weight w : locations.values()) {
                collection.add(new WeightedLatLng(new LatLng(Double.parseDouble(w.lat), Double.parseDouble(w.lot)), w.intesity));
            }

            return collection;
        } catch (Exception ex) {
            return null;
        } finally {
            if (reader != null)
                try {
                    reader.close();
                } catch (IOException e) {

                }
        }
    }*/

    public String toString() {
        String str = "MAC Adresses: ";
        ArrayList<String> temp;
        for (int i = 0; i < MacAdressList.size(); ++i)
            str += MacAdressList.get(i) + " ";

        str += "\nLocations\n";
        for (String location : LocationRSS_HashMap.keySet()) {
            str += location + " ";
            temp = LocationRSS_HashMap.get(location);
            for (int i = 0; i < temp.size(); ++i)
                str += temp.get(i) + " ";
            str += "\n";
        }

        return str;
    }

    public String four()
    {
        return "4";
    }

}
