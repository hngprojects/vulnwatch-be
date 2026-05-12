package com.vulnwatch.worker.scanners.dns;

import com.vulnwatch.worker.enums.SurfaceType;
import com.vulnwatch.worker.enums.TargetType;
import com.vulnwatch.worker.interfaces.Scanner;
import com.vulnwatch.worker.models.ScanJob;
import com.vulnwatch.worker.models.ScanResult;
import com.vulnwatch.worker.scanners.dns.models.ScanContext;
import com.vulnwatch.worker.scanners.dns.utility.DnsResolver;
import com.vulnwatch.worker.scanners.dns.utility.RuleEngine;
import java.util.Map;
import lombok.RequiredArgsConstructor;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;
import org.xbill.DNS.*;

@Service
@RequiredArgsConstructor
public class DnsScanner implements Scanner {

  /**
   * Performs a DNS scan for the given domain. Beginners: This is where we check for MX, TXT, and A
   * records.
   */
  private final DnsResolver dnsResolver;

  private final RuleEngine ruleEngine;

  @Override
  @Cacheable()
  public ScanResult scan(ScanJob job) {
    try {
      ScanContext scanContext = dnsResolver.resolveRecords(job.getDomain()).join();
      Map<String, Object> scanResult = ruleEngine.scanJob(scanContext);
      return ScanResult.success(job.getScanId(), "DNS_SCANNER", SurfaceType.DNS, scanResult);
    } catch (Exception e) {
      return ScanResult.failure(job.getScanId(), "DNS_SCANNER", SurfaceType.DNS, e);
    }
  }

  @Override
  public TargetType getTargetType() {
    return TargetType.DOMAIN;
  }

  @Override
  public SurfaceType getSurfaceType() {
    return SurfaceType.DNS;
  }
}
