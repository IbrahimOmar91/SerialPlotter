void setup() {
  Serial.begin(9600);
}

void loop() {
  Serial.print(random(1000));
  Serial.print(',');
  Serial.print(random(1000));
  Serial.print(',');
  Serial.print(random(1000));
  Serial.print(',');
  Serial.print(random(1000));
  Serial.print(',');
  Serial.print(random(1000));
  Serial.print(',');
  Serial.println(random(1000));
  delay(10);
}
