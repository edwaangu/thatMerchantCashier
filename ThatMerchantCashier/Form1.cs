using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace ThatMerchantCashier
{
    public partial class Form1 : Form
    {
        // The number of potions, rope, or arrows a user decides to order.
        double numOfPotions = 0;
        double numOfRope = 0;
        double numOfArrows = 0;

        // The set prices of those three products
        double potionPrice = 3;
        double ropePrice = 0.10;
        double arrowPrice = 0.05;

        // Important total calculating variables
        double preOrderCost = 0;
        double taxRate = 0.13;
        double taxAmount = 0;
        double totalOrderCost = 0;

        /* Variables that will display the amount of bronze, silver, and gold required to pay this. */
        // Copper gets calculated by multiplying the number by 100 and getting a remainder of that 100
        double copperPre;
        double copperTax;
        double copperTotal;

        // Silver gets calculated by getting a remainder of the one and ten digits
        double silverPre;
        double silverTax;
        double silverTotal;

        // Gold gets calculated by dividing by 100 and rounding down.
        double goldPre;
        double goldTax;
        double goldTotal;

        // Platinum gets calculated by dividing by 10,000 and rounding down.
        double platinumPre;
        double platinumTax;
        double platinumTotal;

        // These are used to hold the amounts that the user entered in the change boxes
        double copperBoxAmt = 0;
        double silverBoxAmt = 0;
        double goldBoxAmt = 0;
        double platinumBoxAmt = 0;

        // These are used to hold values that will be displayed on screen near the change area
        double copperChange = 0;
        double silverChange = 0;
        double goldChange = 0;
        double platinumChange = 0;

        // These are used to calculate your funds entered and the change remaining
        double totalFunds;
        double calculateChange;

        // These are used for receipt
        Random nextRandom = new Random(); // (I do like looking ahead a bit and finding useful things)
        int orderNumber;
        int terrariaDay;
        bool receiptPrinting = false;
        double receiptY = 0;

        // This is the sound object for the receipt
        SoundPlayer printerSound = new SoundPlayer(Properties.Resources.printerNoise);

        // This function is used to reset the cashier UI when something happens
        public void resetCashier()
        {
            // Reset the text in the potion, rope, and arrow amounts
            amtBox1.Text = "0";
            amtBox2.Text = "0";
            amtBox3.Text = "0";

            // Re-enable the amount boxes if they were disabled
            amtBox1.Enabled = true;
            amtBox2.Enabled = true;
            amtBox3.Enabled = true;

            // Re-enable the 'calculate totals' button
            calculateTotalButton.Enabled = true;

            // Hide the copper, silver, gold, and platinum coins from the total price part
            copperCoin1.Visible = false;
            copperCoin2.Visible = false;
            copperCoin3.Visible = false;

            silverCoin1.Visible = false;
            silverCoin2.Visible = false;
            silverCoin3.Visible = false;

            goldCoin1.Visible = false;
            goldCoin2.Visible = false;
            goldCoin3.Visible = false;

            platinumCoin1.Visible = false;
            platinumCoin2.Visible = false;
            platinumCoin3.Visible = false;

            // Hide the labels in the same area
            copperLabels.Text = "";
            silverLabels.Text = "";
            goldLabels.Text = "";
            platinumLabels.Text = "";
            infoText.Visible = false;

            // Disable the remaining buttons and the fill in boxes for change
            newOrderButton.Enabled = false;
            printReceiptButton.Enabled = false;
            calculateChangeButton.Enabled = false;
            platinumBox.Enabled = false;
            goldBox.Enabled = false;
            silverBox.Enabled = false;
            copperBox.Enabled = false;

            // Reset the change boxes to contain 0
            platinumBox.Text = "0";
            goldBox.Text = "0";
            silverBox.Text = "0";
            copperBox.Text = "0";
            copperBoxAmt = 0;
            silverBoxAmt = 0;
            goldBoxAmt = 0;
            platinumBoxAmt = 0;

            // Reset the variables that calculate the total costs
            totalFunds = 0;
            preOrderCost = 0;
            totalOrderCost = 0;

            // Hide the coins and labels from the funds input section
            copperCoin5.Visible = false;
            silverCoin5.Visible = false;
            goldCoin5.Visible = false;
            platinumCoin5.Visible = false;

            copperChangeLabel.Visible = false;
            silverChangeLabel.Visible = false;
            goldChangeLabel.Visible = false;
            platinumChangeLabel.Visible = false;

            copperChangeLabel.Text = "0";
            silverChangeLabel.Text = "0";
            goldChangeLabel.Text = "0";
            platinumChangeLabel.Text = "0";

            // Re-setup the hidden receipt sizes
            receiptOutputTop.Size = new Size(350, 0);
            receiptOutput.Size = new Size(241, 0);
            receiptOutputRight.Size = new Size(350, 0);
            receiptOutputBottom.Size = new Size(350, 0);

            // Reset the 'Y-Position' of the receipt
            receiptY = 0;

            // Hide the 'Change' text and disable the 'New Order' button
            changeAmt.Visible = false;
            newOrderButton.Enabled = false;

            // When the program runs the first time, the program overwrites this dialogue, but the second time round it will display this text instead
            merchantDialog.Text = "Welcome back!";
        }

        public Form1()
        {
            InitializeComponent();
            // Initialize with resetting the Cashier
            resetCashier(); 

            // Replace the merchant's dialogue, and start the global program timer
            merchantDialog.Text = "Welcome!\n\nHere we use a different currency, which consists of types of coins, a silver is equal to $1. While gold, platinum, and copper are $10000, $100, and $0.01 respectively.";
            receiptTimer.Start();
            receiptTimer.Enabled = true;
        }

        private void calculateTotalButton_Click(object sender, EventArgs e)
        {
            // Attempt to run this function's code, if the user enters letters or other characters that aren't numbers in any of the text boxes, the try function will fail and the catch part will commence
            try
            {
                // Get the number of potions, rope, and arrows the user had inputted
                numOfPotions = Math.Floor(Convert.ToDouble(amtBox1.Text));
                numOfRope = Math.Floor(Convert.ToDouble(amtBox2.Text));
                numOfArrows = Math.Floor(Convert.ToDouble(amtBox3.Text));

                // Stop the user from requesting decimal items
                if (Convert.ToDouble(amtBox1.Text) != numOfPotions || Convert.ToDouble(amtBox2.Text) != numOfRope || Convert.ToDouble(amtBox3.Text) != numOfArrows)
                {
                    resetCashier();
                    copperLabels.Text = "--\n\n--\n\n--";
                    merchantDialog.Text = "Why would I give you a fraction of an item?";
                    return;
                }

                // Stop the user from entering negative items
                if (Math.Abs(numOfPotions) != numOfPotions || Math.Abs(numOfRope) != numOfRope || Math.Abs(numOfArrows) != numOfArrows)
                {
                    resetCashier();
                    copperLabels.Text = "--\n\n--\n\n--";
                    merchantDialog.Text = "I'm not going to pay you!";
                    return;
                }

                // Stop the user from purchasing nothing
                if (numOfPotions < 1 && numOfRope < 1 && numOfArrows < 1)
                {
                    resetCashier();
                    copperLabels.Text = "--\n\n--\n\n--";
                    merchantDialog.Text = "I recommend purchasing at least one item.";
                    return;
                }

                // Calculate the pre order cost by multiplying each amount by its value, and add the three together
                preOrderCost = (potionPrice * numOfPotions) + (ropePrice * numOfRope) + (arrowPrice * numOfArrows);
                taxAmount = preOrderCost * taxRate; // Calculate taxes by multiplying the pre-order by the tax rate
                totalOrderCost = preOrderCost + taxAmount + 0.01; // Calculate the total by adding the pre-order and taxes together

                // Calculate bronze amounts
                copperPre = Math.Floor((preOrderCost * 100) % 100);
                copperTax = Math.Floor((taxAmount * 100) % 100);
                copperTotal = Math.Floor((totalOrderCost * 100) % 100);

                // Calculate silver amounts
                silverPre = Math.Floor(preOrderCost % 100);
                silverTax = Math.Floor(taxAmount % 100);
                silverTotal = Math.Floor(totalOrderCost % 100);

                // Calculate gold amounts
                goldPre = Math.Floor((preOrderCost / 100) % 100);
                goldTax = Math.Floor((taxAmount / 100) % 100);
                goldTotal = Math.Floor((totalOrderCost / 100) % 100);

                // Calculate platinum amounts
                platinumPre = Math.Floor((preOrderCost / 10000));
                platinumTax = Math.Floor((taxAmount / 10000));
                platinumTotal = Math.Floor((totalOrderCost / 10000));

                // Reset the receipt and show the informational text saying stuff like 'Pre-order, Taxes, and Total'
                receiptY = 0;
                infoText.Visible = true;

                // Show the copper labels and coin images
                copperLabels.Text = $"{copperPre}\n\n{copperTax}\n\n{copperTotal}";
                copperCoin1.Visible = true;
                copperCoin2.Visible = true;
                copperCoin3.Visible = true;

                // Check multiple if statements if it's necessary to show certain silver coins and labels
                silverLabels.Text = "";
                silverCoin1.Visible = false;
                silverCoin2.Visible = false;
                silverCoin3.Visible = false;
                if (silverPre > 0 || goldPre > 0 || platinumPre > 0){   
                    silverLabels.Text += $"{silverPre}";
                    silverCoin1.Visible = true;
                }
                else { silverLabels.Text += ""; }
                silverLabels.Text += "\n\n";
                if (silverTax > 0 || goldTax > 0 || platinumTax > 0) { 
                    silverLabels.Text += $"{silverTax}";
                    silverCoin2.Visible = true;
                }
                else { silverLabels.Text += ""; }
                silverLabels.Text += "\n\n";
                if (silverTotal > 0 || goldTotal > 0 || platinumTotal > 0) { 
                    silverLabels.Text += $"{silverTotal}";
                    silverCoin3.Visible = true;
                }

                // Check multiple if statements if it's necessary to show certain gold coins and labels
                goldLabels.Text = "";
                goldCoin1.Visible = false;
                goldCoin2.Visible = false;
                goldCoin3.Visible = false;
                if (goldPre > 0 || platinumPre > 0)
                {
                    goldLabels.Text += $"{goldPre}";
                    goldCoin1.Visible = true;
                }
                else { goldLabels.Text += ""; }
                goldLabels.Text += "\n\n";
                if (goldTax > 0 || platinumTax > 0)
                {
                    goldLabels.Text += $"{goldTax}";
                    goldCoin2.Visible = true;
                }
                else { goldLabels.Text += ""; }
                goldLabels.Text += "\n\n";
                if (goldTotal > 0 || platinumTotal > 0)
                {
                    goldLabels.Text += $"{goldTotal}";
                    goldCoin3.Visible = true;
                }


                // Check multiple if statements if it's necessary to show certain platinum coins and labels
                platinumLabels.Text = "";
                platinumCoin1.Visible = false;
                platinumCoin2.Visible = false;
                platinumCoin3.Visible = false;
                if (platinumPre > 0)
                {
                    platinumLabels.Text += $"{platinumPre}";
                    platinumCoin1.Visible = true;
                }
                else { platinumLabels.Text += ""; }
                platinumLabels.Text += "\n\n";
                if (platinumTax > 0)
                {
                    platinumLabels.Text += $"{platinumTax}";
                    platinumCoin2.Visible = true;
                }
                else { platinumLabels.Text += ""; }
                platinumLabels.Text += "\n\n";
                if (platinumTotal > 0)
                {
                    platinumLabels.Text += $"{platinumTotal}";
                    platinumCoin3.Visible = true;
                }
                receiptOutput.Text = "";

                // Reset the 'Input funds' category of items so the user doesn't get away by paying less
                platinumBox.Text = "0";
                goldBox.Text = "0";
                silverBox.Text = "0";
                copperBox.Text = "0";
                printReceiptButton.Enabled = false;
                copperCoin5.Visible = false;
                silverCoin5.Visible = false;
                goldCoin5.Visible = false;
                platinumCoin5.Visible = false;
                copperChangeLabel.Visible = false;
                silverChangeLabel.Visible = false;
                goldChangeLabel.Visible = false;
                platinumChangeLabel.Visible = false;
                receiptY = 0;
                changeAmt.Visible = false;

                // Allow the user to type in the 'Insert Funds' text boxes, and allow the user to calculate their change
                copperBox.Enabled = true;
                silverBox.Enabled = true;
                goldBox.Enabled = true;
                platinumBox.Enabled = true;

                calculateChangeButton.Enabled = true;

                // Offer unique dialog based on how many items the user wants to purchase
                merchantDialog.Text = "Sounds great, how much are you willing to pay?";
                if (numOfPotions == 30 && numOfRope == 999 && numOfArrows == 999)
                {
                    merchantDialog.Text = "The usual, eh?";
                }
                if (numOfPotions == 9999 && numOfRope == 999999 && numOfArrows == 9999999)
                {
                    merchantDialog.Text = "Good luck affording that.";
                }
            }
            catch
            {
                // Reset all cashier variables and add a warning dialogue
                resetCashier();
                copperLabels.Text = "--\n\n--\n\n--";
                merchantDialog.Text = "I can't recognize that amount!";
            }
        }

        private void calculateChangeButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Set up variables housing info on how much the user has inputed
                platinumBoxAmt = Math.Floor(Convert.ToDouble(platinumBox.Text));
                goldBoxAmt = Math.Floor(Convert.ToDouble(goldBox.Text));
                silverBoxAmt = Math.Floor(Convert.ToDouble(silverBox.Text));
                copperBoxAmt = Math.Floor(Convert.ToDouble(copperBox.Text));

                // If the user enters a negative number in the 'funds' boxes, reset those values
                if(platinumBoxAmt < 0 || goldBoxAmt < 0 || silverBoxAmt < 0 || copperBoxAmt < 0)
                {
                    copperCoin5.Visible = false;
                    silverCoin5.Visible = false;
                    goldCoin5.Visible = false;
                    platinumCoin5.Visible = false;
                    copperChangeLabel.Visible = false;
                    silverChangeLabel.Visible = false;
                    goldChangeLabel.Visible = false;
                    platinumChangeLabel.Visible = false;
                    printReceiptButton.Enabled = false;
                    platinumBox.Text = "0";
                    goldBox.Text = "0";
                    silverBox.Text = "0";
                    copperBox.Text = "0";
                    merchantDialog.Text = "You can't enter negative numbers!";
                    return;
                }

                // Reset the receipt again
                receiptY = 0;

                // Set up a totalFunds variable, and convert the coin values to a single number
                totalFunds = 0;

                totalFunds += platinumBoxAmt * 10000;
                totalFunds += goldBoxAmt * 100;
                totalFunds += silverBoxAmt;
                totalFunds += copperBoxAmt / 100;

                // Decide if the user has paid enough money
                if(totalFunds < totalOrderCost - 0.01)
                {
                    // If they don't have enough, hide the change hud and tell the user to try again
                    copperCoin5.Visible = false;
                    silverCoin5.Visible = false;
                    goldCoin5.Visible = false;
                    platinumCoin5.Visible = false;
                    copperChangeLabel.Visible = false;
                    silverChangeLabel.Visible = false;
                    goldChangeLabel.Visible = false;
                    platinumChangeLabel.Visible = false;
                    printReceiptButton.Enabled = false;
                    receiptY = 0;
                    merchantDialog.Text = "You don't have enough for that!";
                    return;
                }

                // Calculate the amount of change by subtracting the order cost from the funds
                calculateChange = totalFunds - (totalOrderCost - 0.01);

                // Show the change displaying hud
                copperCoin5.Visible = true;
                copperChangeLabel.Visible = true;
                if (calculateChange >= 1)
                {
                    silverCoin5.Visible = true;
                    silverChangeLabel.Visible = true;
                }
                if (calculateChange >= 100)
                {
                    goldCoin5.Visible = true;
                    goldChangeLabel.Visible = true;
                }
                if (calculateChange >= 10000)
                {
                    platinumCoin5.Visible = true;
                    platinumChangeLabel.Visible = true;
                }
                
                // Convert back from single number to multiple
                copperChange = Math.Floor((Convert.ToDouble(calculateChange) * 100) % 100);
                silverChange = Math.Floor((Convert.ToDouble(calculateChange)) % 100);
                goldChange = Math.Floor((Convert.ToDouble(calculateChange) / 100) % 100);
                platinumChange = Math.Floor((Convert.ToDouble(calculateChange) / 10000) % 100);

                // Display the amounts of change remaining
                copperChangeLabel.Text = $"{copperChange}";
                silverChangeLabel.Text = $"{silverChange}";
                goldChangeLabel.Text = $"{goldChange}";
                platinumChangeLabel.Text = $"{platinumChange}";

                // Allow the user to see the 'Change' label, and allow the user to print out their receipt
                printReceiptButton.Enabled = true;
                changeAmt.Visible = true;

                // More merchant dialog, with exclusive dialogue if the user inputs 99 into every slot.
                merchantDialog.Text = "Guess it's time to print out the receipt.";
                if(totalFunds == 999999.99)
                {
                    merchantDialog.Text = "How are you so rich?";
                }
            }
            catch
            {
                // Reset and hide the change hud, along with some dialogue if the user enters some invalid numbers
                platinumBox.Text = "0";
                goldBox.Text = "0";
                silverBox.Text = "0";
                copperBox.Text = "0";
                printReceiptButton.Enabled = false;
                copperCoin5.Visible = false;
                silverCoin5.Visible = false;
                goldCoin5.Visible = false;
                platinumCoin5.Visible = false;
                copperChangeLabel.Visible = false;
                silverChangeLabel.Visible = false;
                goldChangeLabel.Visible = false;
                platinumChangeLabel.Visible = false;
                receiptY = 0;
                changeAmt.Visible = false;
                merchantDialog.Text = "I don't recognize that amount.";

            }
        }

        private void printReceiptButton_Click(object sender, EventArgs e)
        {
            // Decide on a random order number and date
            orderNumber = nextRandom.Next(1, 100001);
            terrariaDay = nextRandom.Next(1, 10001);

            /*
            - receiptOutput (Left) consists of text like 'Sub-Total', 'Terrarian Taxes', 'Rope', as well as the amount of items purchased
            - receiptOutputTop consists of 'That One Merchant', 'Customer #______', and 'Day _____'
            - receiptOutputRight consists of the values of items purchased, total costs, taxes, funds, and change
            - receiptOutputBottom consists of a single line of text thanking the user
            */ // Create the base text, consisting of the order and day
            receiptOutput.Text = "\n\n\n  =========================\n\n";
            receiptOutputTop.Text = $"            That One Merchant\nCustomer #{orderNumber}\nDay {terrariaDay}\n\n";
            receiptOutputRight.Text = "\n\n\n\n\n\n";

            // Decide on how many types of items it'll show on the receipt
            if(numOfPotions > 0)
            {
                receiptOutput.Text += $"Potions  x{numOfPotions}\n";
                receiptOutputRight.Text += $"{(potionPrice * numOfPotions).ToString("C")}\n";
            }
            if (numOfRope > 0)
            {
                receiptOutput.Text += $"Rope     x{numOfRope}\n";
                receiptOutputRight.Text += $"{(ropePrice * numOfRope).ToString("C")}\n";
            }
            if (numOfArrows > 0)
            {
                receiptOutput.Text += $"Arrows   x{numOfArrows}\n";
                receiptOutputRight.Text += $"{(arrowPrice * numOfArrows).ToString("C")}\n";
            }

            // Show the totals and changes
            receiptOutput.Text += "  =========================\n\nSubtotal";
            receiptOutputRight.Text += $"\n\n\n{(preOrderCost).ToString("C")}";
            receiptOutput.Text += "\nTerrarian Taxes";
            receiptOutputRight.Text += $"\n{(taxAmount).ToString("C")}";
            receiptOutput.Text += "\nTotal";
            receiptOutputRight.Text += $"\n{(totalOrderCost - 0.01).ToString("C")}";
            receiptOutput.Text += "\n  =========================\n\nFunds";
            receiptOutputRight.Text += $"\n\n\n\n{(totalFunds).ToString("C")}";
            receiptOutput.Text += "\nChange";
            receiptOutputRight.Text += $"\n{(calculateChange).ToString("C")}";
            receiptOutput.Text += "\n  =========================";

            // Set up the receipt printation by disabling all buttons and text boxes
            receiptY = 0;
            receiptPrinting = true;
            printReceiptButton.Enabled = false;
            calculateChangeButton.Enabled = false;
            calculateTotalButton.Enabled = false;
            platinumBox.Enabled = false;
            goldBox.Enabled = false;
            silverBox.Enabled = false;
            copperBox.Enabled = false;
            amtBox1.Enabled = false;
            amtBox2.Enabled = false;
            amtBox3.Enabled = false;

            // Have rare chances of lucky dialogue, and start the printer sound (Which is just a slowed down version of falling down on thin ice in the game Terraria)
            printerSound.Play();
            merchantDialog.Text = "I love the sound of the receipt printer!";
            if(terrariaDay == 10000)
            {
                merchantDialog.Text = "Last day already?";
            }
            if (orderNumber == 1)
            {
                merchantDialog.Text = "You're my first customer!";
            }
            if (orderNumber == 1000)
            {
                merchantDialog.Text = "You're my thousandth customer!";
            }
            if (orderNumber == 100000)
            {
                merchantDialog.Text = "You're my hundred-thousandth customer! Congratulations.";
            }
        }

        private void newOrderButton_Click(object sender, EventArgs e)
        {
            // When the user wants to start a new order, treat it as resetting the entire program basically.
            resetCashier();
        }

        private void receiptTimer_Tick(object sender, EventArgs e)
        {
            // receiptTimer constantly ticks, so it needs to be stored in an if statement so it doesn't keep extending when it's not printing
            if (receiptPrinting)
            {
                // Change it by 1.5 to remain in sync with the printer sound (May be off, but apparently this is as fast as my computer can run it) Either 1.5 or 1.
                receiptY+=1.5;

                // Since receiptY is a double, it has to convert it to an integer so decimal numbers don't mess everything up, and the Size object only allows for integer numbers
                if(Convert.ToInt32(receiptY) < 81)
                { // Keep printing out the top receipt part until 81 pixels
                    receiptOutputTop.Size = new Size(350, Convert.ToInt32(receiptY));
                }

                // Always keep printing out the left and right sides of the receipt
                receiptOutput.Size = new Size(241, Convert.ToInt32(receiptY));
                receiptOutputRight.Size = new Size(350, Convert.ToInt32(receiptY));

                // Only start printing out the bottom receipt part after 428 pixels down.
                if(Convert.ToInt32(receiptY) >= 428)
                {
                    receiptOutputBottom.Size = new Size(350, (Convert.ToInt32(receiptY) - 428));
                }
                else
                {
                    receiptOutputBottom.Size = new Size(350, 0);
                }

                // When it gets to the bottom of the receipt, stop printing, change a few values, and update the text.
                if (Convert.ToInt32(receiptY) >= 500)
                {
                    receiptY = 0;
                    receiptPrinting = false;
                    newOrderButton.Enabled = true;
                    merchantDialog.Text = "Thanks for bargaining with me!";
                }
            }
        }
    }
}
