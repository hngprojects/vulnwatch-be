package com.vulnwatch.worker.scantests;

import static org.junit.jupiter.api.Assertions.assertNotNull;

import com.vulnwatch.worker.models.ScanJob;
import com.vulnwatch.worker.models.ScanResult;
import com.vulnwatch.worker.scanners.dns.DnsScanner;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

@SpringBootTest
public class ScannerTest {

  @Autowired private DnsScanner dnsScanner;

  @Test
  public void scanJob() {
    ScanJob job = ScanJob.builder().scanId(UUID.randomUUID()).domain("google.com").build();

    ScanResult scanResult = dnsScanner.scan(job);

    assertNotNull(scanResult);
  }
}
